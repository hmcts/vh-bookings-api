using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewVenueHattonCrossTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: nameof(HearingVenue),
               columns: new[] { "Id", "Name" },
               values: new object[,]
               {
                    {29, "Hatton Cross Tribunal Hearing Centre"}
               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 29);
        }
    }
}
