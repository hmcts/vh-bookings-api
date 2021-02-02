using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateSeedData_JudgeUserRoleId : Migration
    {
        private const int HearingRoleId = 13;
        private const int oldUserRoleId = 5;
        private const int newUserRoleId = 4;

        protected void UpdateHearingRoleUserRoleIdForJudge(MigrationBuilder builder, int hearingRoleId, int newValue)
        {
            builder.Sql($"UPDATE HearingRole SET UserRoleId = { newValue } WHERE Id = { hearingRoleId }");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateHearingRoleUserRoleIdForJudge(migrationBuilder, HearingRoleId, newUserRoleId);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpdateHearingRoleUserRoleIdForJudge(migrationBuilder, HearingRoleId, oldUserRoleId);
        }
    }
}
