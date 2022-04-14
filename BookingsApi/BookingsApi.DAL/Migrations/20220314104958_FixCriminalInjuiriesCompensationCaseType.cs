using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class FixCriminalInjuiriesCompensationCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[HearingRole] set [UserRoleId] = 7 where [Id] = 989 and [CaseRoleId] = 311");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[HearingRole] set [UserRoleId] = 7 where [Id] = 989 and [CaseRoleId] = 311");
        }
    }
}
