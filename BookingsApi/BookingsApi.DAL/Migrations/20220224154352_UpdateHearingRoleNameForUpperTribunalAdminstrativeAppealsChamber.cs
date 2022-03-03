using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateHearingRoleNameForUpperTribunalAdminstrativeAppealsChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[HearingRole] set [Name] = 'Traffic Commissioners Officer' where [Id] = 912 and [CaseRoleId] = 293 and [UserRoleId] = 5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update [dbo].[HearingRole] set [Name] = 'Traffic Commissioners Officer' where [Id] = 912 and [CaseRoleId] = 293 and [UserRoleId] = 5");
        }
    }
}
