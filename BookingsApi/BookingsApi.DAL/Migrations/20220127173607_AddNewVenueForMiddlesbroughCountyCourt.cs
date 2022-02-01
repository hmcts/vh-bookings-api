using Microsoft.EntityFrameworkCore.Migrations;
using BookingsApi.Domain;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewVenueForMiddlesbroughCountyCourt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    {23, "Middlesbrough County Court"}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 23);
        }
    }
}
