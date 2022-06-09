using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddLeaverAndLeftOnColumnsToJudiciaryPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Leaver",
                table: "JudiciaryPerson",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LeftOn",
                table: "JudiciaryPerson",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Leaver",
                table: "JudiciaryPerson");

            migrationBuilder.DropColumn(
                name: "LeftOn",
                table: "JudiciaryPerson");
        }
    }
}
