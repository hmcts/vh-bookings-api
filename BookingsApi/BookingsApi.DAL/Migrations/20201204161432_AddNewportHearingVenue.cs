using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewportHearingVenue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 14, "Newport" }
                });    
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 14);
        }
    }
}
