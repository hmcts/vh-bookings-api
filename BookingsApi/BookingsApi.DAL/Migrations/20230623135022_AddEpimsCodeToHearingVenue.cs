using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddEpimsCodeToHearingVenue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EpimsCode",
                table: "HearingVenue",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HearingVenue_EpimsCode",
                table: "HearingVenue",
                column: "EpimsCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HearingVenue_EpimsCode",
                table: "HearingVenue");

            migrationBuilder.DropColumn(
                name: "EpimsCode",
                table: "HearingVenue");
        }
    }
}
