using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class FixCaseTypeUpperTribunalImmigrationAndAsylumChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[CaseRole] set [Group] = 0 where [Id] = 285 and CaseTypeId = 45 ");  // Judge case role is 0
            migrationBuilder.Sql("update [dbo].[CaseRole] set [Group] = 6 where [Id] = 286 and CaseTypeId = 45 ");   // Panel Member case role is 6
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[CaseRole] set [Group] = 0 where [Id] = 285 and CaseTypeId = 45 ");  // Judge case role is 0
            migrationBuilder.Sql("update [dbo].[CaseRole] set [Group] = 6 where [Id] = 286 and CaseTypeId = 45 ");   // Panel Member case role is 6
        }
    }
}
