using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class FixCaseRoleForCaseTypeUpperTribunalImmigrationAndAsylumChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[CaseRole] set [Group] = 13 where [Id] = 287 and CaseTypeId = 45 ");  // Secretary of State case role is 13
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[CaseRole] set [Group] = 13 where [Id] = 287 and CaseTypeId = 45 ");  // Secretary of State case role is 13
        }
    }
}
