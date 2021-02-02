using Bookings.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class New_Hearing_Venue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    {9, "Bristol Civil and Family Justice Centre"},
                    {10, "Southampton Hearing Centre"}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 9);
            migrationBuilder.DeleteData("HearingVenue", "Id", 10);
        }
    }
}
