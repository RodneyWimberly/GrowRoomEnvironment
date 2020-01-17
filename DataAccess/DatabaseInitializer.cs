using GrowRoomEnvironment.Core.Logging;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess
{
    public class DatabaseInitializer
    {
        private readonly ILogger _logger;
        private readonly PersistedGrantDbContext _persistedGrantContext;
        private readonly ConfigurationDbContext _configurationContext;
        private readonly ApplicationDbContext _applicationContext;
        private readonly IAccountManager _accountManager;

        public DatabaseInitializer(ILogger<DatabaseInitializer> logger, PersistedGrantDbContext persistedGrantContext, ConfigurationDbContext configurationContext, ApplicationDbContext applicationContext, IAccountManager accountManager)
        {
            _logger = logger;
            _persistedGrantContext = persistedGrantContext;
            _configurationContext = configurationContext;
            _applicationContext = applicationContext;
            _accountManager = accountManager;
        }

        public void InitializeIdentityServerDatabase()
        {
            InitializeIdentityServerDatabaseAsync().Wait();
        }

        public async Task InitializeIdentityServerDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Running PersistedGrantDbContext Migration");
                await _persistedGrantContext.Database.MigrateAsync();

                _logger.LogInformation("Running ConfigurationDbContext Migration");
                await _configurationContext.Database.MigrateAsync();

                if (!await _configurationContext.Clients.AnyAsync())
                {
                    _logger.LogInformation("Generating Identity Server Clients");
                    await _configurationContext.Clients.AddRangeAsync(GetClients().Select(m => m.ToEntity()));
                    await _configurationContext.SaveChangesAsync();
                    _logger.LogInformation("Generating Identity Server Clients Completed");
                }

                if (!await _configurationContext.IdentityResources.AnyAsync())
                {
                    _logger.LogInformation("Generating Identity Server IdentityResources");
                    await _configurationContext.IdentityResources.AddRangeAsync(GetIdentityResources().Select(m => m.ToEntity()));
                    await _configurationContext.SaveChangesAsync();
                    _logger.LogInformation("Generating Identity Server IdentityResources Completed");
                }

                if (!await _configurationContext.ApiResources.AnyAsync())
                {
                    _logger.LogInformation("Generating Identity Server ApiResources");
                    await _configurationContext.ApiResources.AddRangeAsync(GetApiResources().Select(m => m.ToEntity()));
                    await _configurationContext.SaveChangesAsync();
                    _logger.LogInformation("Generating Identity Server ApiResources Completed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);
                throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
            }
        }


        public void InitializeApplicationDatabase()
        {
            InitializeApplicationDatabaseAsync().Wait();
        }

        public async Task InitializeApplicationDatabaseAsync()
        {
            try
            {
                _applicationContext.ChangeTracker.LazyLoadingEnabled = false;

                // Migration
                _logger.LogInformation("Running ApplicationDbContext Migration");
                await _applicationContext.Database.MigrateAsync();

                // Users and Roles
                if (!await _applicationContext.Users.AnyAsync())
                {
                    _logger.LogInformation("Generating sample accounts");

                    const string adminRoleName = "administrator";
                    const string userRoleName = "user";

                    await EnsureRoleAsync(_accountManager, adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                    await EnsureRoleAsync(_accountManager, userRoleName, "Default user", new string[] { });

                    await CreateUserAsync(_accountManager, "Manager", "admin", "P@55w0rd", "Sample Administrator User", "admin@wimberlytech.com", "+1 (123) 555-1212", new string[] { adminRoleName });
                    await CreateUserAsync(_accountManager, "Worker", "user", "P@55w0rd", "Sample Standard User", "user@wimberlytech.com", "+1 (123) 555-1212", new string[] { userRoleName });

                    _logger.LogInformation("Sample account generation completed");
                }

                // DataPoints
                if (!await _applicationContext.DataPoints.AnyAsync())
                {
                    _logger.LogInformation("Generating DataPoints");

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


                    await _applicationContext.DataPoints.AddAsync(dp_1);
                    await _applicationContext.DataPoints.AddAsync(dp_2);
                    await _applicationContext.DataPoints.AddAsync(dp_3);
                    await _applicationContext.DataPoints.AddAsync(dp_4);
                    await _applicationContext.DataPoints.AddAsync(dp_5);
                    await _applicationContext.DataPoints.AddAsync(dp_6);
                    await _applicationContext.DataPoints.AddAsync(dp_7);
                    await _applicationContext.DataPoints.AddAsync(dp_8);
                    await _applicationContext.DataPoints.AddAsync(dp_9);
                    await _applicationContext.DataPoints.AddAsync(dp_10);
                    await _applicationContext.SaveChangesAsync();
                    _logger.LogInformation("Seeding DataPoints completed");
                }

                // Notifications
                if (!await _applicationContext.Notifications.AnyAsync())
                {
                    _logger.LogInformation("Generating Notifications");

                    await _applicationContext.Notifications.AddAsync(new Notification
                    {
                        Header = "Action Failure",
                        Body = "The light failed to turn on at the scheduled time",
                        IsPinned = false,
                        IsRead = false,
                        Date = DateTime.UtcNow
                    });

                    await _applicationContext.Notifications.AddAsync(new Notification
                    {
                        Header = "Sensor Read Failure",
                        Body = "Failed to read the air temperature sensor",
                        IsPinned = false,
                        IsRead = false,
                        Date = DateTime.UtcNow
                    });

                    await _applicationContext.Notifications.AddAsync(new Notification
                    {
                        Header = "Sensor Read Failure",
                        Body = "Failed to read the CO2 PPM sensor",
                        IsPinned = false,
                        IsRead = true,
                        Date = DateTime.UtcNow
                    });

                    await _applicationContext.SaveChangesAsync();
                    _logger.LogInformation("Seeding Notifications completed");
                }

                // ActionDevices
                if (!await _applicationContext.ActionDevices.AnyAsync())
                {
                    _logger.LogInformation("Generating ActionDevices");
                    await _applicationContext.ActionDevices.AddAsync(new ActionDevice
                    {
                        Name = "Light",
                        Type = ActionDeviceTypes.X10,
                        Parameters = "A1",
                        IsEnabled = true
                    });
                    await _applicationContext.ActionDevices.AddAsync(new ActionDevice
                    {
                        Name = "CO2",
                        Type = ActionDeviceTypes.X10,
                        Parameters = "A2",
                        IsEnabled = true
                    });
                    await _applicationContext.ActionDevices.AddAsync(new ActionDevice
                    {
                        Name = "Exhaust",
                        Type = ActionDeviceTypes.X10,
                        Parameters = "A3",
                        IsEnabled = true
                    });
                    await _applicationContext.SaveChangesAsync();
                    _logger.LogInformation("Seeding ActionDevice completed");
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.INIT_DATABASE, ex, LoggingEvents.INIT_DATABASE.Name);
                throw new Exception(LoggingEvents.INIT_DATABASE.Name, ex);
            }
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
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

        public static IEnumerable<ApiResource> GetApiResources()
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

        public static IEnumerable<Client> GetClients()
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

        private async Task EnsureRoleAsync(IAccountManager accountManager, string roleName, string description, string[] claims)
        {
            if ((await accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                (bool Succeeded, string[] Errors) result = await accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(IAccountManager accountManager, string jobTitle, string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
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
