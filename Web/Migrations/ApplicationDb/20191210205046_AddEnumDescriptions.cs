using Microsoft.EntityFrameworkCore.Migrations;

namespace GrowRoomEnvironment.Web.Migrations.ApplicationDb
{
    public partial class AddEnumDescriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LevelDescription",
                table: "AppLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateDescription",
                table: "AppEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatorDescription",
                table: "AppEventConditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeDescription",
                table: "AppActionDevices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
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
                table: "AppActionDevices");
        }
    }
}
