using System;
using System.Threading.Tasks;
using GrowRoomEnvironment.Contracts.DataAccess;
using GrowRoomEnvironment.DataAccess.Core;
using GrowRoomEnvironment.DataAccess.Core.Interfaces;
using GrowRoomEnvironment.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GrowRoomEnvironment.DataAccess
{

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;

        public DatabaseInitializer(ApplicationDbContext applicationDbContext, 
            IAccountManager accountManager, ILogger<DatabaseInitializer> logger)
        {
            _accountManager = accountManager;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            // Migration
            await _applicationDbContext.Database.MigrateAsync().ConfigureAwait(false);
           
            // Users and Roles
            if (!await _applicationDbContext.Users.AnyAsync())
            {
                _logger.LogInformation("Generating sample accounts");

                const string adminRoleName = "administrator";
                const string userRoleName = "user";

                await EnsureRoleAsync(adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                await CreateUserAsync("Manager", "admin", "P@55w0rd", "Sample Administrator User", "admin@wimberlytech.com", "+1 (123) 555-1212", new string[] { adminRoleName });
                await CreateUserAsync("Worker", "user", "P@55w0rd", "Sample Standard User", "user@wimberlytech.com", "+1 (123) 555-1212", new string[] { userRoleName });

                _logger.LogInformation("Sample account generation completed");
            }
        
            // DataPoints
            if (!await _applicationDbContext.DataPoints.AnyAsync())
            {
                _logger.LogInformation("Generating DataPoints");

                DataPoint dp_1 = new DataPoint
                {
                    DataPointId = 1,
                    Caption = "Air Temperature",
                    Icon = "thermostat.jpg",
                    Template = "OffOnValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_2 = new DataPoint
                {
                    DataPointId = 2,
                    Caption = "Air Humidity",
                    Icon = "Humidity.png",
                    Template = "OffOnValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_3 = new DataPoint
                {
                    DataPointId = 3,
                    Caption = "CO2 Parts per million",
                    Icon = "CO2.png",
                    Template = "PPMValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_4 = new DataPoint
                {
                    DataPointId = 4,
                    Caption = "Time",
                    Icon = "Time.png",
                    Template = "OffOnDateTimeTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_5 = new DataPoint
                {
                    DataPointId = 5,
                    Caption = "Lights currently one",
                    Icon = "LightOn.png",
                    Template = "OffOnValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_6 = new DataPoint
                {
                    DataPointId = 6,
                    Caption = "Plant Growth Medium Moisture Level",
                    Icon = "Moisture.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_7 = new DataPoint
                {
                    DataPointId = 7,
                    Caption = "Nutrient pH",
                    Icon = "ph.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_8 = new DataPoint
                {
                    DataPointId = 8,
                    Caption = "Nutrient Parts per million",
                    Icon = "tds.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_9 = new DataPoint
                {
                    DataPointId = 9,
                    Caption = "Nutrient Temperature",
                    Icon = "Temperature.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_10 = new DataPoint
                {
                    DataPointId = 10,
                    Caption = "Nutrient Volume",
                    Icon = "volume.jpg",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };


                _applicationDbContext.DataPoints.Add(dp_1);
                _applicationDbContext.DataPoints.Add(dp_2);
                _applicationDbContext.DataPoints.Add(dp_3);
                _applicationDbContext.DataPoints.Add(dp_4);
                _applicationDbContext.DataPoints.Add(dp_5);
                _applicationDbContext.DataPoints.Add(dp_6);
                _applicationDbContext.DataPoints.Add(dp_7);
                _applicationDbContext.DataPoints.Add(dp_8);
                _applicationDbContext.DataPoints.Add(dp_9);
                _applicationDbContext.DataPoints.Add(dp_10);
                await _applicationDbContext.SaveChangesAsync();
                _logger.LogInformation("Seeding DataPoints completed");
            }

            // Notifications
           // if (!await _applicationDbContext.EnumLookups.AnyAsync())
            //{
            //    _logger.LogInformation("Generating Notifications");

                

            //    await _applicationDbContext.SaveChangesAsync();
            //    _logger.LogInformation("Seeding Notifications completed");
            //}
            
            // EnumLook
            if (!await _applicationDbContext.EnumLookups.AnyAsync())
            {
                _logger.LogInformation("Generating EnumLookups");

                // ActionDeviceStates
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppActionDevice",
                    EnumName = "ActionDeviceStates",
                    EnumValue = 0,
                    EnumDescription = "Off",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppActionDevice",
                    EnumName = "ActionDeviceStates",
                    EnumValue = 1,
                    EnumDescription = "On",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });

                // ActionDeviceTypes
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppActionDevice",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 0,
                    EnumDescription = "X10",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppActionDevice",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 1,
                    EnumDescription = "ZWave",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }); _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppActionDevice",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 2,
                    EnumDescription = "ZeeBee",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppActionDevice",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 3,
                    EnumDescription = "WiFi",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });

                // ErrorLevels
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppLogs",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 1,
                    EnumDescription = "Debug",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppLogs",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 2,
                    EnumDescription = "Information",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }); _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppLogs",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 3,
                    EnumDescription = "Warning",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppLogs",
                    EnumName = "ActionDeviceTypes",
                    EnumValue = 4,
                    EnumDescription = "Error",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });

                // Operators
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppEventConditions",
                    EnumName = "Operators",
                    EnumValue = 0,
                    EnumDescription = "Equal",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppEventConditions",
                    EnumName = "Operators",
                    EnumValue = 1,
                    EnumDescription = "NotEqual",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }); _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppEventConditions",
                    EnumName = "Operators",
                    EnumValue = 2,
                    EnumDescription = "GreaterThan",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                _applicationDbContext.EnumLookups.Add(new EnumLookup
                {
                    Table = "AppEventConditions",
                    EnumName = "Operators",
                    EnumValue = 3,
                    EnumDescription = "LessThan",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                });
                await _applicationDbContext.SaveChangesAsync();
                _logger.LogInformation("Seeding EnumLookups completed");
            }
        }

        private async Task EnsureRoleAsync(string roleName, string description, string[] claims)
        {
            if ((await _accountManager.GetRoleByNameAsync(roleName)) == null)
            {
                ApplicationRole applicationRole = new ApplicationRole(roleName, description);

                (bool Succeeded, string[] Errors) result = await this._accountManager.CreateRoleAsync(applicationRole, claims);

                if (!result.Succeeded)
                    throw new Exception($"Seeding \"{description}\" role failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string jobTitle, string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
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

            (bool Succeeded, string[] Errors) result = await _accountManager.CreateUserAsync(applicationUser, roles, password);

            if (!result.Succeeded)
                throw new Exception($"Seeding \"{userName}\" user failed. Errors: {string.Join(Environment.NewLine, result.Errors)}");


            return applicationUser;
        }
    }
}
