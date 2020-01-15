using Microsoft.EntityFrameworkCore.Migrations;

namespace GrowRoomEnvironment.Web.Migrations.ApplicationDb
{
    public partial class AddIsEnabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "Caption",
                table: "AppDataPoints");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "AppDataPoints");

            migrationBuilder.DropColumn(
                name: "Template",
                table: "AppDataPoints");*/

            migrationBuilder.RenameColumn(
                name: "ShowInUI",
                table: "AppDataPoints",
                newName: "IsEnabled");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AppEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AppDataPoints",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AppActionDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AppEvents");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AppDataPoints");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AppActionDevices");

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "AppDataPoints",
                newName: "ShowInUI");

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "AppDataPoints",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "AppDataPoints",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Template",
                table: "AppDataPoints",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
