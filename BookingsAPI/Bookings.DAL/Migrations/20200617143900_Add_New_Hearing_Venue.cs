using Bookings.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
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

        }
    }
}
