using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomReservation.API.Migrations
{
    public partial class NormalizeRecurringExceptionDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExceptionDates",
                table: "RecurringGroups");

            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "RecurringGroups",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "RecurringGroupExceptionDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecurringGroupId = table.Column<int>(type: "int", nullable: false),
                    ExceptionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringGroupExceptionDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringGroupExceptionDates_RecurringGroups_RecurringGroupId",
                        column: x => x.RecurringGroupId,
                        principalTable: "RecurringGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 18, 51, 42, 148, DateTimeKind.Utc).AddTicks(8537));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 18, 51, 42, 148, DateTimeKind.Utc).AddTicks(8567));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 18, 51, 42, 148, DateTimeKind.Utc).AddTicks(8568));

            migrationBuilder.CreateIndex(
                name: "IX_RecurringGroupExceptionDates_RecurringGroupId_ExceptionDate",
                table: "RecurringGroupExceptionDates",
                columns: new[] { "RecurringGroupId", "ExceptionDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringGroupExceptionDates");

            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "RecurringGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "ExceptionDates",
                table: "RecurringGroups",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 17, 3, 19, 57, 662, DateTimeKind.Local).AddTicks(814));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 17, 3, 19, 57, 662, DateTimeKind.Local).AddTicks(823));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 17, 3, 19, 57, 662, DateTimeKind.Local).AddTicks(824));
        }
    }
}
