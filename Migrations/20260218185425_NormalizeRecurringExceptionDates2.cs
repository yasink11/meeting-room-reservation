using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoomReservation.API.Migrations
{
    public partial class NormalizeRecurringExceptionDates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 18, 54, 25, 213, DateTimeKind.Utc).AddTicks(144));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 18, 54, 25, 213, DateTimeKind.Utc).AddTicks(148));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 2, 18, 18, 54, 25, 213, DateTimeKind.Utc).AddTicks(149));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
