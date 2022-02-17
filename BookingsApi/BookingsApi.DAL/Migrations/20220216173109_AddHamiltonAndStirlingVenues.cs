using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddHamiltonAndStirlingVenues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: nameof(HearingVenue),
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                {30, "Hamilton Brandon Gate"},
                {31, "Stirling Wallace House"}
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 30);
            migrationBuilder.DeleteData("HearingVenue", "Id", 31);
        }
    }
}
