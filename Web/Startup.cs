using AutoMapper;
using GrowRoomEnvironment.Contracts.DataAccess;
using GrowRoomEnvironment.Contracts.Email;
using GrowRoomEnvironment.Contracts.Logging;
using GrowRoomEnvironment.Core;
using GrowRoomEnvironment.Core.Email;
using GrowRoomEnvironment.Core.Logging;
using GrowRoomEnvironment.DataAccess;
using GrowRoomEnvironment.DataAccess.Core;
using GrowRoomEnvironment.DataAccess.Core.Constants;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Models;
using GrowRoomEnvironment.Web.Authorization;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using Serilog;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Http;

namespace GrowRoomEnvironment.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectString = Configuration["ConnectionStrings:DefaultConnection"];
            string migrationsAssembly = GetType().Assembly.GetName().Name;

            // Logging
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            // EF
            services.AddDbContext<ApplicationDbContext>(dbOptions =>
                dbOptions.UseSqlite(connectString,
                sqliteOptions => sqliteOptions.MigrationsAssembly(migrationsAssembly)));

            // UnitOfWorkTransactionFactory
            DbContextOptionsBuilder<ApplicationDbContext> dbContextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connectString,
                sqliteOptions => sqliteOptions.MigrationsAssembly(migrationsAssembly));
            services.AddSingleton(new UnitOfWorkTransactionFactory(dbContextBuilder.Options));

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                        
            services.Configure<IdentityOptions>(Configuration.GetSection("IdentityOptions"));

            // Adds IdentityServer.
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlite(connectString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlite(connectString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();


            string applicationUrl = Configuration["ApplicationUrl"].TrimEnd('/');

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = applicationUrl;
                    options.SupportedTokens = SupportedTokens.Jwt;
                    options.RequireHttpsMetadata = false; // Note: Set to true in production
                    options.ApiName = IdentityServerConfigurationExtensions.ApiName;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.ViewAllUsersPolicy, policy => policy.RequireClaim(Claims.Permission, ApplicationPermissions.ViewUsers));
                options.AddPolicy(Policies.ManageAllUsersPolicy, policy => policy.RequireClaim(Claims.Permission, ApplicationPermissions.ManageUsers));

                options.AddPolicy(Policies.ViewAllRolesPolicy, policy => policy.RequireClaim(Claims.Permission, ApplicationPermissions.ViewRoles));
                options.AddPolicy(Policies.ViewRoleByRoleNamePolicy, policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()));
                options.AddPolicy(Policies.ManageAllRolesPolicy, policy => policy.RequireClaim(Claims.Permission, ApplicationPermissions.ManageRoles));

                options.AddPolicy(Policies.AssignAllowedRolesPolicy, policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));
            });


            // Add cors
            services.AddCors();

            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            services.AddValidatorsFromAssembly(GetType().Assembly);
            services.AddMvc().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = IdentityServerConfigurationExtensions.ApiFriendlyName, Version = "v1" });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>()
                            {
                                { IdentityServerConfigurationExtensions.ApiName, IdentityServerConfigurationExtensions.ApiFriendlyName }
                            }
                        }
                    }
                });
            });

            services.AddAutoMapper(typeof(Startup));

            // Configurations
            services.Configure<SmtpConfig>(Configuration.GetSection("SmtpConfig"));


            // Business Services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILoggerService, LoggerService>();

            // Repositories
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();
            services.AddScoped<IAccountManager, AccountManager>();

            // Auth Handlers
            services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();

            // DB Creation and Seeding
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger, IDatabaseInitializer databaseInitializer)
        {
            StoragePath.Initialize(env);
            //loggerFactory.AddEntityFramework<ApplicationDbContext>(app.ApplicationServices,);
            //loggerFactory.AddFile(Configuration.GetSection("Logging"));
            //loggerFactory.AddSerilog(new LoggerConfiguration()
            //    .WriteTo.SQLite(StoragePath.DbFile)
            //    .CreateLogger());


            EmailTemplates.Initialize(env);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            try
            {
                app.InitializeIdentityServerDatabase();
                databaseInitializer.SeedAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);
                throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseCors(builder => builder
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Swagger UI - AngularIot.Web";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{IdentityServerConfigurationExtensions.ApiFriendlyName} V1");
                c.OAuthClientId(IdentityServerConfigurationExtensions.SwaggerClientID);
                c.OAuthClientSecret("no_password"); //Leaving it blank doesn't work
            });

            //app.UseCookiePolicy();
            //app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    spa.Options.StartupTimeout = TimeSpan.FromSeconds(120); // Increase the timeout if angular app is taking longer to startup
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // Use this instead to use the angular cli server

                }
            });

           
        }

       
    }
}
