using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddVenuesForSSCSAndEAT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 49, "Edinburgh Employment Appeal Tribunal" },
                    { 50, "Inverness Justice Centre" },
                    { 51, "Edinburgh Social Security and Child Support Tribunal" },
                    { 52, "Edinburgh Upper Tribunal (Administrative Appeals Chamber)" },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingVenueId = 49; hearingVenueId <= 52; hearingVenueId++)
            {
                migrationBuilder.DeleteData(nameof(HearingVenue), "Id", hearingVenueId);
            }
        }
    }
}
