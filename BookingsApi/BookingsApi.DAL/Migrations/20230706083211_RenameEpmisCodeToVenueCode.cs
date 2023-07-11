using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class RenameEpmisCodeToVenueCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EpimsCode",
                table: "HearingVenue",
                newName: "VenueCode");

            migrationBuilder.RenameIndex(
                name: "IX_HearingVenue_EpimsCode",
                table: "HearingVenue",
                newName: "IX_HearingVenue_VenueCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VenueCode",
                table: "HearingVenue",
                newName: "EpimsCode");

            migrationBuilder.RenameIndex(
                name: "IX_HearingVenue_VenueCode",
                table: "HearingVenue",
                newName: "IX_HearingVenue_EpimsCode");
        }
    }
}
