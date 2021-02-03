using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddJudicialOfficeHolderHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE HearingRole SET UserRoleId = 7 
                WHERE Name = 'Panel Member' OR Name = 'Winger'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE HearingRole SET UserRoleId = 5 
                WHERE Name = 'Panel Member' OR Name = 'Winger'");
        }
    }
}