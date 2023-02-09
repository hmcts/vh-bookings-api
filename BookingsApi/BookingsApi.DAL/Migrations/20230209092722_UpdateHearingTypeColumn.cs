using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateHearingTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(HearingType),
                keyColumn: "Id",
                keyValue: 284,
                column: "Name",
                value: "Interlocutory/Directions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(HearingType),
                keyColumn: "Id",
                keyValue: 284,
                column: "Name",
                value: "Interloculatory/Directions");
        }
    }
}
