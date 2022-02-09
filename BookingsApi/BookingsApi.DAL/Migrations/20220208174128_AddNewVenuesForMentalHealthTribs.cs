using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewVenuesForMentalHealthTribs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: nameof(HearingVenue),
               columns: new[] { "Id", "Name" },
               values: new object[,]
               {
                    {24, "Mary Seacole House - Birmingham"},
                    {25, "Goodmayes Hospital - Essex"},
                    {26, "Huntercombe Roehampton Hospital"},
                    {27, "Ardenleigh - Birmingham"}
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingVenueId = 24; hearingVenueId <= 27; hearingVenueId++)
            {
                migrationBuilder.DeleteData(nameof(HearingVenue), "Id", hearingVenueId);
            }
        }
    }
}
