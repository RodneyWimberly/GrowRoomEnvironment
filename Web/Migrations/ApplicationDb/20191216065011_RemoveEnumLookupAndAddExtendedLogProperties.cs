using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GrowRoomEnvironment.Web.Migrations.ApplicationDb
{
    public partial class RemoveEnumLookupAndAddExtendedLogProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEnumLookups");

            migrationBuilder.AddColumn<string>(
                name: "Cookies",
                table: "AppLogs",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormVariables",
                table: "AppLogs",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "AppLogs",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueryString",
                table: "AppLogs",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerVariables",
                table: "AppLogs",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "AppLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cookies",
                table: "AppLogs");

            migrationBuilder.DropColumn(
                name: "FormVariables",
                table: "AppLogs");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "AppLogs");

            migrationBuilder.DropColumn(
                name: "QueryString",
                table: "AppLogs");

            migrationBuilder.DropColumn(
                name: "ServerVariables",
                table: "AppLogs");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "AppLogs");

            migrationBuilder.CreateTable(
                name: "AppEnumLookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnumDescription = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    EnumName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EnumValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Table = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEnumLookups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEnumLookups_EnumName",
                table: "AppEnumLookups",
                column: "EnumName");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnumLookups_Id",
                table: "AppEnumLookups",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEnumLookups_Table",
                table: "AppEnumLookups",
                column: "Table");
        }
    }
}
