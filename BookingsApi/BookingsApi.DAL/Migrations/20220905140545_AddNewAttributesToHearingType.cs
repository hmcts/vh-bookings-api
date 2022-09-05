using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewAttributesToHearingType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "HearingType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelshName",
                table: "HearingType",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "HearingType");

            migrationBuilder.DropColumn(
                name: "WelshName",
                table: "HearingType");
        }
    }
}
