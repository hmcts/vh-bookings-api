using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class RemoveLocalAuthorityRoleForEmploymentAppealTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingRole SET Live = 0 WHERE Id IN (922, 936)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingRole SET Live = 1 WHERE Id IN (922, 936)");
        }
    }
}
