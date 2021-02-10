using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class add_hearing_venues_for_court_of_appeal_criminal_division : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    {11, "Royal Courts of Justice"},
                    {12, "Crown Court"}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 12);
            migrationBuilder.DeleteData("HearingVenue", "Id", 11);
        }
    }
}
