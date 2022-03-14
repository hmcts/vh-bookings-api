using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateWarPensionAppeals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingRole SET UserRoleId = 7 WHERE Id = 1008");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingRole SET UserRoleId = 5 WHERE Id = 1008");
        }
    }
}
