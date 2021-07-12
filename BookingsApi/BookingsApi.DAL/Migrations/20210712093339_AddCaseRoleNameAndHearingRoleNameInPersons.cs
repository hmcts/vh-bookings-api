using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseRoleNameAndHearingRoleNameInPersons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaseRoleName",
                table: "Person",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HearingRoleName",
                table: "Person",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseRoleName",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "HearingRoleName",
                table: "Person");
        }
    }
}
