using Arch.EntityFrameworkCore.UnitOfWork;
using GrowRoomEnvironment.DataAccess.Core;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using System;
using System.Globalization;
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
            services.AddScoped<IRepositoryFactory, HttpUnitOfWork>();
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();
            services.AddScoped<IUnitOfWork<ApplicationDbContext>, HttpUnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfigurationSection identityOptionsConfig,
            string connectString, bool enableSensitiveDataLogging = false, bool useLazyLoadingProxies = false)

        {
            string migrationsAssembly = typeof(DatabaseExtensions).Assembly.FullName;

            // EF
            services.AddDbContext<ApplicationDbContext>(dbOptions =>
            {
                dbOptions.UseSqlite(connectString, sqliteOptions => sqliteOptions.MigrationsAssembly(migrationsAssembly));
                dbOptions.EnableSensitiveDataLogging(enableSensitiveDataLogging);
                dbOptions.UseLazyLoadingProxies(useLazyLoadingProxies);
            });

            // add identity system for users and roles
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure IdentityOptions
            IdentityModelEventSource.ShowPII = enableSensitiveDataLogging;
            services.Configure<IdentityOptions>(io => io = identityOptionsConfig.Get<IdentityOptions>());

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
            services.AddScoped<DatabaseInitializer>();
            return services;
        }

        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            return app.InitializeDatabaseAsync().Result;
        }

        public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                DatabaseInitializer databaseInitializer = serviceScope.ServiceProvider.GetService<DatabaseInitializer>();

                await databaseInitializer.InitializeApplicationDatabaseAsync();
                await databaseInitializer.InitializeIdentityServerDatabaseAsync();
            }
            return app;
        }

        public static IIdentityServerBuilder UseInMemoryStore(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(DatabaseInitializer.GetIdentityResources())
                .AddInMemoryApiResources(DatabaseInitializer.GetApiResources())
                .AddInMemoryClients(DatabaseInitializer.GetClients());
            return builder;
        }
    }
}
