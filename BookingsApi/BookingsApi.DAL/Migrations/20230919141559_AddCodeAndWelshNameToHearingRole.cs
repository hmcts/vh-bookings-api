using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCodeAndWelshNameToHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "HearingRole",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelshName",
                table: "HearingRole",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HearingRole_Code",
                table: "HearingRole",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HearingRole_Code",
                table: "HearingRole");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "HearingRole");

            migrationBuilder.DropColumn(
                name: "WelshName",
                table: "HearingRole");
        }
    }
}
