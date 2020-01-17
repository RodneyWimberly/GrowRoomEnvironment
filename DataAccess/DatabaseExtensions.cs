using Arch.EntityFrameworkCore.UnitOfWork;
using GrowRoomEnvironment.DataAccess.Core;
using GrowRoomEnvironment.DataAccess.Core.Constants;
using GrowRoomEnvironment.DataAccess.Core.Enums;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public static class DatabaseExtensions
    {
        public static void SetupAuditableEntityProperties<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class, IAuditableEntity
        {
            entityTypeBuilder.Property(e => e.CreatedBy).HasColumnType("TEXT");
            entityTypeBuilder.Property(e => e.CreatedDate).HasColumnType("TEXT").HasConversion(v => v.ToUniversalTime().ToString("o", CultureInfo.CurrentCulture), v => DateTime.Parse(v, null, DateTimeStyles.AssumeUniversal));
            entityTypeBuilder.Property(e => e.UpdatedBy).HasColumnType("TEXT");
            entityTypeBuilder.Property(e => e.UpdatedDate).HasColumnType("TEXT").HasConversion(v => v.ToUniversalTime().ToString("o", CultureInfo.CurrentCulture), v => DateTime.Parse(v, null, DateTimeStyles.AssumeUniversal));
        }

        public static void SetupConcurrencyTrackingEntityProperties<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class, IConcurrencyTrackingEntity
        {
            entityTypeBuilder.Property(e => e.RowVersion).HasColumnType("BLOB").IsRowVersion();
        }

        public static IServiceCollection AddHttpUnitOfWork(this IServiceCollection services)

        {
            services.AddTransient<IRepositoryFactory, HttpUnitOfWork>();
            services.AddTransient<IUnitOfWork, HttpUnitOfWork>();
            services.AddTransient<IUnitOfWork<ApplicationDbContext>, HttpUnitOfWork>();
            return services;
        }

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfigurationSection identityOptionsConfig,
            string connectString, string migrationsAssembly, bool enableSensitiveDataLogging = false, bool useLazyLoadingProxies = false)

        {
            // EF
            services.AddDbContext<ApplicationDbContext>(dbOptions =>
            {
                dbOptions.UseSqlite(connectString, sqliteOptions => sqliteOptions.MigrationsAssembly(migrationsAssembly));
                dbOptions.EnableSensitiveDataLogging(enableSensitiveDataLogging);
                dbOptions.UseLazyLoadingProxies(useLazyLoadingProxies);
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            // add identity system for users and roles
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure IdentityOptions
            IdentityModelEventSource.ShowPII = enableSensitiveDataLogging;
            services.Configure<IdentityOptions>(identityOptionsConfig);

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
                    options.TokenCleanupInterval = 300;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();

            services.AddScoped<IAccountManager, AccountManager>();

            return services;
        }

        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            return app.InitializeDatabaseAsync().Result;
        }

        public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
        {
            await app.InitializeApplicationDatabaseAsync();
            await app.InitializeIdentityServerDatabaseAsync();
            return app;
        }

        public static IApplicationBuilder InitializeIdentityServerDatabase(this IApplicationBuilder app)
        {
            return app.InitializeIdentityServerDatabaseAsync().Result;
        }

        public static async Task<IApplicationBuilder> InitializeIdentityServerDatabaseAsync(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                ILogger logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<PersistedGrantDbContext>>();

                logger.LogInformation("Running PersistedGrantDbContext Migration");
                await serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

                logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ConfigurationDbContext>>();
                ConfigurationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                logger.LogInformation("Running ConfigurationDbContext Migration");
                await context.Database.MigrateAsync();

                if (!await context.Clients.AnyAsync())
                {
                    logger.LogInformation("Generating Identity Server Clients");
                    await context.Clients.AddRangeAsync(GetClients().Select(m => m.ToEntity()));
                    await context.SaveChangesAsync();
                    logger.LogInformation("Generating Identity Server Clients Completed");
                }

                if (!await context.IdentityResources.AnyAsync())
                {
                    logger.LogInformation("Generating Identity Server IdentityResources");
                    await context.IdentityResources.AddRangeAsync(GetIdentityResources().Select(m => m.ToEntity()));
                    await context.SaveChangesAsync();
                    logger.LogInformation("Generating Identity Server IdentityResources Completed");
                }

                if (!await context.ApiResources.AnyAsync())
                {
                    logger.LogInformation("Generating Identity Server ApiResources");
                    await context.ApiResources.AddRangeAsync(GetApiResources().Select(m => m.ToEntity()));
                    await context.SaveChangesAsync();
                    logger.LogInformation("Generating Identity Server ApiResources Completed");
                }
            }
            return app;
        }

        public static IApplicationBuilder InitializeApplicationDatabase(this IApplicationBuilder app)
        {
            return app.InitializeApplicationDatabaseAsync().Result;
        }

        public static async Task<IApplicationBuilder> InitializeApplicationDatabaseAsync(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationDbContext.ChangeTracker.LazyLoadingEnabled = false;
                IAccountManager accountManager = serviceScope.ServiceProvider.GetRequiredService<IAccountManager>();
                ILogger logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

                // Migration
                logger.LogInformation("Running ApplicationDbContext Migration");
                await applicationDbContext.Database.MigrateAsync();

                // Users and Roles
                if (!await applicationDbContext.Users.AnyAsync())
                {
                    logger.LogInformation("Generating sample accounts");

                    const string adminRoleName = "administrator";
                    const string userRoleName = "user";

                    await EnsureRoleAsync(accountManager, adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                    await EnsureRoleAsync(accountManager, userRoleName, "Default user", new string[] { });

                    await CreateUserAsync(accountManager, "Manager", "admin", "P@55w0rd", "Sample Administrator User", "admin@wimberlytech.com", "+1 (123) 555-1212", new string[] { adminRoleName });
                    await CreateUserAsync(accountManager, "Worker", "user", "P@55w0rd", "Sample Standard User", "user@wimberlytech.com", "+1 (123) 555-1212", new string[] { userRoleName });

                    logger.LogInformation("Sample account generation completed");
                }

                // DataPoints
                if (!await applicationDbContext.DataPoints.AnyAsync())
                {
                    logger.LogInformation("Generating DataPoints");

                    DataPoint dp_1 = new DataPoint
                    {
                        Name = "Air Temperature",
                        IsEnabled = true,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_2 = new DataPoint
                    {
                        Name = "Air Humidity",
                        IsEnabled = true,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_3 = new DataPoint
                    {
                        Name = "CO2 Parts per million",
                        IsEnabled = true,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_4 = new DataPoint
                    {
                        Name = "Time",
                        IsEnabled = true,
                        DataType = DataPointDataTypes.Time
                    };

                    DataPoint dp_5 = new DataPoint
                    {
                        Name = "Lights currently one",
                        IsEnabled = true,
                        DataType = DataPointDataTypes.Boolean
                    };

                    DataPoint dp_6 = new DataPoint
                    {
                        Name = "Plant Growth Medium Moisture Level",
                        IsEnabled = false,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_7 = new DataPoint
                    {
                        Name = "Nutrient pH",
                        IsEnabled = false,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_8 = new DataPoint
                    {
                        Name = "Nutrient Parts per million",
                        IsEnabled = false,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_9 = new DataPoint
                    {
                        Name = "Nutrient Temperature",
                        IsEnabled = false,
                        DataType = DataPointDataTypes.Number
                    };

                    DataPoint dp_10 = new DataPoint
                    {
                        Name = "Nutrient Volume",
                        IsEnabled = false,
                        DataType = DataPointDataTypes.Number
                    };


                    await applicationDbContext.DataPoints.AddAsync(dp_1);
                    await applicationDbContext.DataPoints.AddAsync(dp_2);
                    await applicationDbContext.DataPoints.AddAsync(dp_3);
                    await applicationDbContext.DataPoints.AddAsync(dp_4);
                    await applicationDbContext.DataPoints.AddAsync(dp_5);
                    await applicationDbContext.DataPoints.AddAsync(dp_6);
                    await applicationDbContext.DataPoints.AddAsync(dp_7);
                    await applicationDbContext.DataPoints.AddAsync(dp_8);
                    await applicationDbContext.DataPoints.AddAsync(dp_9);
                    await applicationDbContext.DataPoints.AddAsync(dp_10);
                    await applicationDbContext.SaveChangesAsync();
                    logger.LogInformation("Seeding DataPoints completed");
                }

                // Notifications
                if (!await applicationDbContext.Notifications.AnyAsync())
                {
                    logger.LogInformation("Generating Notifications");

                    await applicationDbContext.Notifications.AddAsync(new Notification
                    {
                        Header = "Action Failure",
                        Body = "The light failed to turn on at the scheduled time",
                        IsPinned = false,
                        IsRead = false,
                        Date = DateTime.UtcNow
                    });

                    await applicationDbContext.Notifications.AddAsync(new Notification
                    {
                        Header = "Sensor Read Failure",
                        Body = "Failed to read the air temperature sensor",
                        IsPinned = false,
                        IsRead = false,
                        Date = DateTime.UtcNow
                    });

                    await applicationDbContext.Notifications.AddAsync(new Notification
                    {
                        Header = "Sensor Read Failure",
                        Body = "Failed to read the CO2 PPM sensor",
                        IsPinned = false,
                        IsRead = true,
                        Date = DateTime.UtcNow
                    });

                    await applicationDbContext.SaveChangesAsync();
                    logger.LogInformation("Seeding Notifications completed");
                }

                // ActionDevices
                if (!await applicationDbContext.ActionDevices.AnyAsync())
                {
                    logger.LogInformation("Generating ActionDevices");
                    await applicationDbContext.ActionDevices.AddAsync(new ActionDevice
                    {
                        Name = "Light",
                        Type = ActionDeviceTypes.X10,
                        Parameters = "A1",
                        IsEnabled = true
                    });
                    await applicationDbContext.ActionDevices.AddAsync(new ActionDevice
                    {
                        Name = "CO2",
                        Type = ActionDeviceTypes.X10,
                        Parameters = "A2",
                        IsEnabled = true
                    });
                    await applicationDbContext.ActionDevices.AddAsync(new ActionDevice
                    {
                        Name = "Exhaust",
                        Type = ActionDeviceTypes.X10,
                        Parameters = "A3",
                        IsEnabled = true
                    });
                    await applicationDbContext.SaveChangesAsync();
                    logger.LogInformation("Seeding ActionDevice completed");
                }
            }
            return app;
        }

        public static IIdentityServerBuilder UseInMemoryStore(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryClients(GetClients());
            return builder;
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource(Scopes.Roles, new List<string> { JwtClaimTypes.Role })
            };
        }

        private static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(IdentityServerValues.ApiId) {
                    UserClaims = {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.PhoneNumber,
                        JwtClaimTypes.Role,
                        Claims.Permission
                    }
                }
            };
        }

        private static IEnumerable<Client> GetClients()
        {
            // Clients credentials.
            return new List<Client>
            {
                // http://docs.identityserver.io/en/release/reference/client.html.
                new Client
                {
                    ClientId = IdentityServerValues.ApplicationClientId,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Resource Owner Password Credential grant.
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        Scopes.Roles,
                        IdentityServerValues.ApiId
                    },
                    AllowOfflineAccess = true, // For refresh token.
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    //AccessTokenLifetime = 900, // Lifetime of access token in seconds.
                    //AbsoluteRefreshTokenLifetime = 7200,
                    //SlidingRefreshTokenLifetime = 900,
                },

                new Client
                {
                    ClientId = IdentityServerValues.DocumentationClientId,
                    ClientName = IdentityServerValues.DocumentationClientName,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,

                    AllowedScopes = {
                        IdentityServerValues.ApiId
                    }
                }
            };
        }

        private static async Task EnsureRoleAsync(IAccountManager accountManager, string roleName, string description, string[] claims)
        {
            if ((await accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                (bool Succeeded, string[] Errors) result = await accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }

        private static async Task<ApplicationUser> CreateUserAsync(IAccountManager accountManager, string jobTitle, string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
                JobTitle = jobTitle,
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                IsEnabled = true
            };

            (bool Succeeded, string[] Errors) result = await accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Succeeded)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");


            return applicationUser;
        }

    }
}
