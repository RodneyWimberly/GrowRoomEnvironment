using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GrowRoomEnvironment.Web.Migrations.ApplicationDb
{
    public partial class CreateEventTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppEnumLookups_Id",
                table: "AppEnumLookups");

            migrationBuilder.DropIndex(
                name: "IX_AppDataPoints_DataPointId",
                table: "AppDataPoints");

            migrationBuilder.CreateTable(
                name: "AppActionDevices",
                columns: table => new
                {
                    ActionDeviceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Parameters = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActionDevices", x => x.ActionDeviceId);
                });

            migrationBuilder.CreateTable(
                name: "AppNotifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Header = table.Column<string>(maxLength: 100, nullable: false),
                    Body = table.Column<string>(maxLength: 250, nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    IsPinned = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppNotifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "AppEvents",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ActionDeviceId = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEvents", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_AppEvents_AppActionDevices_ActionDeviceId",
                        column: x => x.ActionDeviceId,
                        principalTable: "AppActionDevices",
                        principalColumn: "ActionDeviceId");
                });

            migrationBuilder.CreateTable(
                name: "AppEventConditions",
                columns: table => new
                {
                    EventConditionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    DataPointId = table.Column<int>(nullable: false),
                    Operator = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEventConditions", x => x.EventConditionId);
                    table.ForeignKey(
                        name: "FK_AppEventConditions_AppDataPoints_DataPointId",
                        column: x => x.DataPointId,
                        principalTable: "AppDataPoints",
                        principalColumn: "DataPointId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AppEventConditions_AppEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "AppEvents",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEnumLookups_Id",
                table: "AppEnumLookups",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDataPoints_DataPointId",
                table: "AppDataPoints",
                column: "DataPointId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppActionDevices_ActionDeviceId",
                table: "AppActionDevices",
                column: "ActionDeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppActionDevices_Type",
                table: "AppActionDevices",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_AppEventConditions_DataPointId",
                table: "AppEventConditions",
                column: "DataPointId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEventConditions_EventConditionId",
                table: "AppEventConditions",
                column: "EventConditionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEventConditions_EventId",
                table: "AppEventConditions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEvents_ActionDeviceId",
                table: "AppEvents",
                column: "ActionDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEvents_EventId",
                table: "AppEvents",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEvents_State",
                table: "AppEvents",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotifications_NotificationId",
                table: "AppNotifications",
                column: "NotificationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEventConditions");

            migrationBuilder.DropTable(
                name: "AppNotifications");

            migrationBuilder.DropTable(
                name: "AppEvents");

            migrationBuilder.DropTable(
                name: "AppActionDevices");

            migrationBuilder.DropIndex(
                name: "IX_AppEnumLookups_Id",
                table: "AppEnumLookups");

            migrationBuilder.DropIndex(
                name: "IX_AppDataPoints_DataPointId",
                table: "AppDataPoints");

            migrationBuilder.CreateIndex(
                name: "IX_AppEnumLookups_Id",
                table: "AppEnumLookups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppDataPoints_DataPointId",
                table: "AppDataPoints",
                column: "DataPointId");
        }
    }
}
