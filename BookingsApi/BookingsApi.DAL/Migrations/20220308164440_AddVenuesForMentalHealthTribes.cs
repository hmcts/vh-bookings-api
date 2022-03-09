using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddVenuesForMentalHealthTribes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: nameof(HearingVenue),
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                {32, "Mental Health Tribunal"}
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 32);
        }
    }
}
