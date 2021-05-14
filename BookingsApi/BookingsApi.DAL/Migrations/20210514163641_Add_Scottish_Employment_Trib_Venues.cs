using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class Add_Scottish_Employment_Trib_Venues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingVenue),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {15, "Aberdeen Tribunal Hearing Centre"},
                    {16, "Dundee Tribunal Hearing Centre"},
                    {17, "Edinburgh Employment Tribunal"},
                    {18, "Glasgow Tribunals Centre"},
                    {19, "Inverness Employment Tribunal"},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingVenue", "Id", 15);
            migrationBuilder.DeleteData("HearingVenue", "Id", 16);
            migrationBuilder.DeleteData("HearingVenue", "Id", 17);
            migrationBuilder.DeleteData("HearingVenue", "Id", 18);
            migrationBuilder.DeleteData("HearingVenue", "Id", 19);
        }
    }
}
