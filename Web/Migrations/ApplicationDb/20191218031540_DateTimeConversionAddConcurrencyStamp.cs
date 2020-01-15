using Microsoft.EntityFrameworkCore.Migrations;

namespace GrowRoomEnvironment.Web.Migrations.ApplicationDb
{
    public partial class DateTimeConversionAddConcurrencyStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "LevelDescription",
                table: "AppLogs");

            migrationBuilder.DropColumn(
                name: "StateDescription",
                table: "AppEvents");

            migrationBuilder.DropColumn(
                name: "OperatorDescription",
                table: "AppEventConditions");

            migrationBuilder.DropColumn(
                name: "TypeDescription",
                table: "AppActionDevices");*/

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppEventConditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppDataPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppActionDevices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
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
                table: "AppActionDevices");

            migrationBuilder.AddColumn<string>(
                name: "LevelDescription",
                table: "AppLogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateDescription",
                table: "AppEvents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatorDescription",
                table: "AppEventConditions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeDescription",
                table: "AppActionDevices",
                type: "TEXT",
                nullable: true);
        }
    }
}
