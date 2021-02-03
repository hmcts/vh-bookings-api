using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCourtOfAppealCriminalDivisionHearingRoles : Migration
    {
        private const int HearingRoleId = 129;
        private const int oldUserRoleId = 6;
        private const int newUserRoleId = 5;

        protected void UpdateUserRoleForHearingRole(MigrationBuilder builder, int hearingRoleId, int newValue)
        {
            builder.Sql($"UPDATE HearingRole SET UserRoleId = { newValue } WHERE Id = { hearingRoleId }");
        }
        protected void UpdateHearingRoleNameForHearingRole(MigrationBuilder builder, int hearingRoleId, string newValue)
        {
            builder.Sql($"UPDATE HearingRole SET [Name] = '{ newValue }' WHERE Id = { hearingRoleId }");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 175, "Respondent", 5, 88 },
                    { 176, "Respondent Advocate", 6, 88 },
                    { 177, "Special Counsel", 5, 88 },
                });

            UpdateUserRoleForHearingRole(migrationBuilder, HearingRoleId, newUserRoleId);

            UpdateHearingRoleNameForHearingRole(migrationBuilder, 128, "App");
            UpdateHearingRoleNameForHearingRole(migrationBuilder, 130, "App Advocate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 175; hearingRoleID <= 177; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }

            UpdateUserRoleForHearingRole(migrationBuilder, HearingRoleId, oldUserRoleId);

            UpdateHearingRoleNameForHearingRole(migrationBuilder, 128, "Appellant");
            UpdateHearingRoleNameForHearingRole(migrationBuilder, 130, "Defence Advocate");
        }
    }
}
