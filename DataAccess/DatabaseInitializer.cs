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
        private readonly ApplicationDbContext _context;
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;

        public DatabaseInitializer(ApplicationDbContext context, IAccountManager accountManager, ILogger<DatabaseInitializer> logger)
        {
            _accountManager = accountManager;
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            // Migration
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            // Users and Roles
            if (!await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Generating inbuilt accounts");

                const string adminRoleName = "administrator";
                const string userRoleName = "user";

                await EnsureRoleAsync(adminRoleName, "Default administrator", ApplicationPermissions.GetAllPermissionValues());
                await EnsureRoleAsync(userRoleName, "Default user", new string[] { });

                await CreateUserAsync("admin", "P@55w0rd", "Inbuilt Administrator", "admin@elevatormanagement.com", "+1 (123) 000-0000", new string[] { adminRoleName });
                await CreateUserAsync("user", "P@55w0rd", "Inbuilt Standard User", "user@elevatormanagement.com", "+1 (123) 000-0001", new string[] { userRoleName });

                _logger.LogInformation("Inbuilt account generation completed");
            }
        
            // DataPoints
            if (!await _context.DataPoints.AnyAsync())
            {
                _logger.LogInformation("Generating DataPoints");

                DataPoint dp_1 = new DataPoint
                {
                    Id = 1,
                    Caption = "Air Temperature",
                    Icon = "thermostat.jpg",
                    Template = "OffOnValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_2 = new DataPoint
                {
                    Id = 2,
                    Caption = "Air Humidity",
                    Icon = "Humidity.png",
                    Template = "OffOnValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_3 = new DataPoint
                {
                    Id = 3,
                    Caption = "CO2 Parts per million",
                    Icon = "CO2.png",
                    Template = "PPMValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_4 = new DataPoint
                {
                    Id = 4,
                    Caption = "Time",
                    Icon = "Time.png",
                    Template = "OffOnDateTimeTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_5 = new DataPoint
                {
                    Id = 5,
                    Caption = "Lights currently one",
                    Icon = "LightOn.png",
                    Template = "OffOnValueTemplate",
                    ShowInUI = true,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_6 = new DataPoint
                {
                    Id = 6,
                    Caption = "Plant Growth Medium Moisture Level",
                    Icon = "Moisture.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_7 = new DataPoint
                {
                    Id = 7,
                    Caption = "Nutrient pH",
                    Icon = "ph.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_8 = new DataPoint
                {
                    Id = 8,
                    Caption = "Nutrient Parts per million",
                    Icon = "tds.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_9 = new DataPoint
                {
                    Id = 9,
                    Caption = "Nutrient Temperature",
                    Icon = "Temperature.png",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                DataPoint dp_10 = new DataPoint
                {
                    Id = 10,
                    Caption = "Nutrient Volume",
                    Icon = "volume.jpg",
                    Template = "LevelTemplate",
                    ShowInUI = false,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };


                _context.DataPoints.Add(dp_1);
                _context.DataPoints.Add(dp_2);
                _context.DataPoints.Add(dp_3);
                _context.DataPoints.Add(dp_4);
                _context.DataPoints.Add(dp_5);
                _context.DataPoints.Add(dp_6);
                _context.DataPoints.Add(dp_7);
                _context.DataPoints.Add(dp_8);
                _context.DataPoints.Add(dp_9);
                _context.DataPoints.Add(dp_10);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeding DataPoints completed");
            }

            /*
            //ElevatorConfigurations
            if (!await _context.ElevatorConfigurations.AnyAsync())
            {
                _logger.LogInformation("Generating ElevatorConfigurations");

                ElevatorConfiguration elevatorConfiguration = new ElevatorConfiguration
                {
                    NumberOfElevators = 4,
                    NumberOfFloors = 6,
                    TravelTimePerFloor = 5,
                    DestinationWaitTime = 2,
                    RestFloorType = RestFloorTypes.MostUsedOriginFloor,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };
                _context.ElevatorConfigurations.Add(elevatorConfiguration);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeding ElevatorConfigurations completed");
            }
            */

            // EnumLook
            if (!await _context.EnumLookups.AnyAsync())
            {
                _logger.LogInformation("Generating EnumLookups");

    
                EnumLookup el1 = new EnumLookup
                {
                    Table = "Customer",
                    EnumName = "Gender",
                    EnumValue = 0,
                    EnumDescription = "None",
                    CreatedDate  = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                EnumLookup el2 = new EnumLookup
                {
                    Table = "Customer",
                    EnumName = "Gender",
                    EnumValue = 1,
                    EnumDescription = "Female",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                EnumLookup el3 = new EnumLookup
                {
                    Table = "Customer",
                    EnumName = "Gender",
                    EnumValue = 4,
                    EnumDescription = "Male",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _context.EnumLookups.Add(el1);
                _context.EnumLookups.Add(el2);
                _context.EnumLookups.Add(el3);

                await _context.SaveChangesAsync();
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

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password, string fullName, string email, string phoneNumber, string[] roles)
        {
            ApplicationUser applicationUser = new ApplicationUser
            {
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
