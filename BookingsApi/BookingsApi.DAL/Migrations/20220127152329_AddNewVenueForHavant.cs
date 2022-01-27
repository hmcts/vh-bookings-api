using Microsoft.EntityFrameworkCore.Migrations;
using BookingsApi.Domain;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewVenueForHavant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {22, "Havant Justice Centre"}
                });
         }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 22);
        }
    }
}
