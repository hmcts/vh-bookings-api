using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddUniqueMappingsForCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HearingVenue_VenueCode",
                table: "HearingVenue");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "HearingType",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HearingVenue_VenueCode",
                table: "HearingVenue",
                column: "VenueCode",
                unique: true,
                filter: "[VenueCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HearingType_Code",
                table: "HearingType",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HearingVenue_VenueCode",
                table: "HearingVenue");

            migrationBuilder.DropIndex(
                name: "IX_HearingType_Code",
                table: "HearingType");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "HearingType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HearingVenue_VenueCode",
                table: "HearingVenue",
                column: "VenueCode");
        }
    }
}
