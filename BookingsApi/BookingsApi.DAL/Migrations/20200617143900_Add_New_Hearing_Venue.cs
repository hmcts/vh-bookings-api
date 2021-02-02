using Bookings.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class Add_New_Hearing_Venue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               nameof(HearingVenue),
               new[] { "Id", "Name" },
               new object[,]
               {
                    { 4, "London Property Tribunal" },
                    { 5, "Eastern Property Tribunal" },
                    { 6, "Midlands Property Tribunal" },
                    { 7, "Northern Property Tribunal" },
                    { 8, "Southern Property Tribunal" },
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 4);
            migrationBuilder.DeleteData("HearingVenue", "Id", 5);
            migrationBuilder.DeleteData("HearingVenue", "Id", 6);
            migrationBuilder.DeleteData("HearingVenue", "Id", 7);
            migrationBuilder.DeleteData("HearingVenue", "Id", 8);
        }
    }
}
