using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddedCaseTypePropertiesLiveAndServiceId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Live",
                table: "CaseType",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "CaseType",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Live",
                table: "CaseType");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "CaseType");
        }
    }
}
