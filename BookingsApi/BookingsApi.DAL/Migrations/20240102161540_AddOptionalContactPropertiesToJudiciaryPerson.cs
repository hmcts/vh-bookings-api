using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddOptionalContactPropertiesToJudiciaryPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGeneric",
                table: "JudiciaryPerson",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "JudiciaryParticipant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactTelephone",
                table: "JudiciaryParticipant",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGeneric",
                table: "JudiciaryPerson");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "JudiciaryParticipant");

            migrationBuilder.DropColumn(
                name: "ContactTelephone",
                table: "JudiciaryParticipant");
        }
    }
}
