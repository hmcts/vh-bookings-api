using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateAutomatedTestHearingTypeCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingType SET Code = 'AutomatedTest' WHERE Name = 'Automated Test'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingType SET Code = NULL WHERE Name = 'Automated Test'");
        }
    }
}
