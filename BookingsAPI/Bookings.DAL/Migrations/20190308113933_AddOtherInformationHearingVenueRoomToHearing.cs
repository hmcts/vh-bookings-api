using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddOtherInformationHearingVenueRoomToHearing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "HearingRoomName", table: "Hearing", nullable: true);
            migrationBuilder.AddColumn<string>(name: "OtherInformation", table: "Hearing", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "HearingRoomName", table: "Hearing");
            migrationBuilder.DropColumn(name: "OtherInformation", table: "Hearing");
        }
    }
}
