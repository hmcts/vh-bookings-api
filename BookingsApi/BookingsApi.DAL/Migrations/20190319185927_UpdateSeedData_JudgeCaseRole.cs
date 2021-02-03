using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateSeedData_JudgeCaseRole : Migration
    {
        private const int CaseRoleId = 5;
        private const int newGroupId = 0;
        private const int oldGroupId = 3;
        protected void UpdateCaseRoleGroupForJudge(MigrationBuilder builder, int caseRoleId, int newValue)
        {
            builder.Sql($"UPDATE CaseRole SET [Group] = { newValue } WHERE Id = { caseRoleId }");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateCaseRoleGroupForJudge(migrationBuilder, CaseRoleId, newGroupId);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpdateCaseRoleGroupForJudge(migrationBuilder, CaseRoleId, oldGroupId);
        }
    }
}
