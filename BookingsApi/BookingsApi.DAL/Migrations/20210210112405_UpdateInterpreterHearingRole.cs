using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateInterpreterHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[HearingRole] set [Name] = 'Interpreter' where [Name] like 'Inter%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[HearingRole] set [Name] = 'Interpreter ' where [Name] like 'Inter%'");
        }
    }
}
