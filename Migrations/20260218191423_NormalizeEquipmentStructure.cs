using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomReservation.API.Migrations
{
    public partial class NormalizeEquipmentStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecurringGroupExceptionDates_RecurringGroupId_ExceptionDate",
                table: "RecurringGroupExceptionDates");

            migrationBuilder.DropColumn(
                name: "Equipment",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "RecurringGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomEquipments",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomEquipments", x => new { x.EquipmentId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_RoomEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomEquipments_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Projeksiyon" },
                    { 2, "Beyaz Tahta" },
                    { 3, "Telefon" },
                    { 4, "Ses Sistemi" },
                    { 5, "Mikrofon" }
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 19, 14, 22, 971, DateTimeKind.Utc).AddTicks(8197));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 19, 14, 22, 971, DateTimeKind.Utc).AddTicks(8200));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 19, 14, 22, 971, DateTimeKind.Utc).AddTicks(8201));

            migrationBuilder.InsertData(
                table: "RoomEquipments",
                columns: new[] { "EquipmentId", "RoomId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 1 },
                    { 2, 3 },
                    { 3, 1 },
                    { 3, 2 },
                    { 4, 3 },
                    { 5, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringGroupExceptionDates_RecurringGroupId",
                table: "RecurringGroupExceptionDates",
                column: "RecurringGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_Name",
                table: "Equipments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_RoomId",
                table: "RoomEquipments",
                column: "RoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomEquipments");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_RecurringGroupExceptionDates_RecurringGroupId",
                table: "RecurringGroupExceptionDates");

            migrationBuilder.AddColumn<string>(
                name: "Equipment",
                table: "Rooms",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "RecurringGroups",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Equipment" },
                values: new object[] { new DateTime(2026, 2, 18, 18, 54, 25, 213, DateTimeKind.Utc).AddTicks(144), "Projeksiyon,Beyaz Tahta,Telefon" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Equipment" },
                values: new object[] { new DateTime(2026, 2, 18, 18, 54, 25, 213, DateTimeKind.Utc).AddTicks(148), "Projeksiyon,Telefon" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Equipment" },
                values: new object[] { new DateTime(2026, 2, 18, 18, 54, 25, 213, DateTimeKind.Utc).AddTicks(149), "Projeksiyon,Ses Sistemi,Mikrofon,Beyaz Tahta" });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringGroupExceptionDates_RecurringGroupId_ExceptionDate",
                table: "RecurringGroupExceptionDates",
                columns: new[] { "RecurringGroupId", "ExceptionDate" });
        }
    }
}
