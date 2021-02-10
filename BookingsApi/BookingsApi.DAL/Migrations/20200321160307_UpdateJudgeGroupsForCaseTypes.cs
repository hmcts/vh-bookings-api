using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateJudgeGroupsForCaseTypes : Migration
    {
        private const int newJudgeGroupId = 0;
        private const int oldJudgeGroupId = 4;

        protected void UpdateCaseRoleGroupForJudge(MigrationBuilder builder, int caseRoleId, int newValue)
        {
            builder.Sql($"UPDATE CaseRole SET [Group] = { newValue } WHERE Id = { caseRoleId }");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateCaseRoleGroupForJudge(migrationBuilder, 9, newJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 12, newJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 15, newJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 18, newJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 21, newJudgeGroupId);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpdateCaseRoleGroupForJudge(migrationBuilder, 9, oldJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 12, oldJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 15, oldJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 18, oldJudgeGroupId);
            UpdateCaseRoleGroupForJudge(migrationBuilder, 21, oldJudgeGroupId);
        }
    }
}
