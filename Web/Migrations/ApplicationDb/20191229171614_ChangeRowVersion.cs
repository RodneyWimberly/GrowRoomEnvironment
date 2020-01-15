using Microsoft.EntityFrameworkCore.Migrations;

namespace GrowRoomEnvironment.Web.Migrations.ApplicationDb
{
    public partial class ChangeRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppNotifications");

             migrationBuilder.DropColumn(
                 name: "ConcurrencyStamp",
                 table: "AppEvents");

             migrationBuilder.DropColumn(
                 name: "ConcurrencyStamp",
                 table: "AppEventConditions");

             migrationBuilder.DropColumn(
                 name: "ConcurrencyStamp",
                 table: "AppDataPoints");

             migrationBuilder.DropColumn(
                 name: "ConcurrencyStamp",
                 table: "AppActionDevices");*/


            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AspNetUsers",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAspNetUsersRowVersionOnUpdate
                AFTER UPDATE ON AspNetUsers
                BEGIN
                    UPDATE AspNetUsers
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAspNetUsersRowVersionOnInsert
                AFTER INSERT ON AspNetUsers
                BEGIN
                    UPDATE AspNetUsers
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");


            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AspNetRoles",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAspNetRolesRowVersionOnUpdate
                AFTER UPDATE ON AspNetRoles
                BEGIN
                    UPDATE AspNetRoles
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAspNetRolesRowVersionOnInsert
                AFTER INSERT ON AspNetRoles
                BEGIN
                    UPDATE AspNetRoles
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AppNotifications",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppNotificationsRowVersionOnUpdate
                AFTER UPDATE ON AppNotifications
                BEGIN
                    UPDATE AppNotifications
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppNotificationsRowVersionOnInsert
                AFTER INSERT ON AppNotifications
                BEGIN
                    UPDATE AppNotifications
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AppEvents",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppEventsRowVersionOnUpdate
                AFTER UPDATE ON AppEvents
                BEGIN
                    UPDATE AppEvents
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppEventsRowVersionOnInsert
                AFTER INSERT ON AppEvents
                BEGIN
                    UPDATE AppEvents
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AppEventConditions",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppEventConditionsRowVersionOnUpdate
                AFTER UPDATE ON AppEventConditions
                BEGIN
                    UPDATE AppEventConditions
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppEventConditionsRowVersionOnInsert
                AFTER INSERT ON AppEventConditions
                BEGIN
                    UPDATE AppEventConditions
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AppDataPoints",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppDataPointsRowVersionOnUpdate
                AFTER UPDATE ON AppDataPoints
                BEGIN
                    UPDATE AppDataPoints
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppDataPointsRowVersionOnInsert
                AFTER INSERT ON AppDataPoints
                BEGIN
                    UPDATE AppDataPoints
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AppActionDevices",
                rowVersion: true,
                nullable: true);

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppActionDevicesRowVersionOnUpdate
                AFTER UPDATE ON AppActionDevices
                BEGIN
                    UPDATE AppActionDevices
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");

            migrationBuilder.Sql(
            @"
                CREATE TRIGGER SetAppActionDevicesRowVersionOnInsert
                AFTER INSERT ON AppActionDevices
                BEGIN
                    UPDATE AppActionDevices
                    SET RowVersion = randomblob(8)
                    WHERE rowid = NEW.rowid;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AppNotifications");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AppEvents");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AppEventConditions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AppDataPoints");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AppActionDevices");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppNotifications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppEvents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppEventConditions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppDataPoints",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppActionDevices",
                type: "TEXT",
                nullable: true);
        }
    }
}
