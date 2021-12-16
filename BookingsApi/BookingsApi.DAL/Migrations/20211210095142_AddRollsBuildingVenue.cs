using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddRollsBuildingVenue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {20, "Rolls Building"}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 20);
        }
    }
}
