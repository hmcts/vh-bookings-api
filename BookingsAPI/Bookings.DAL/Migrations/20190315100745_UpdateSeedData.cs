using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class UpdateSeedData : Migration
    {
        private const int HearingRoleId = 10;
        private const string HearingRoleName = "McKenzie Friend";
        private const string RespondentLIP = "Respondent LIP";
        private const string ApplicantLIP = "Applicant LIP";

        protected void UpdateHearingRoleForCaseRole(MigrationBuilder builder, int hearingRoleId, string newValue)
        {
            builder.Sql($"UPDATE HearingRole SET Name = '{newValue}' WHERE Id = {hearingRoleId}");
        }

        protected void DeleteHearingRoleMcKenzieFriend(MigrationBuilder builder, string hearingRoleName)
        {
            builder.Sql($"DELETE from HearingRole WHERE Name = '{hearingRoleName}'");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateHearingRoleForCaseRole(migrationBuilder, HearingRoleId, RespondentLIP);
            DeleteHearingRoleMcKenzieFriend(migrationBuilder, HearingRoleName);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            UpdateHearingRoleForCaseRole(migrationBuilder, HearingRoleId, ApplicantLIP);
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "CaseRoleId", "UserRoleId", },
                values: new object[,]
                {
                    // Case role: Claimant
                    {17, "McKenzie Friend", 1, 6},
                    // Case role: Defendant
                    {18, "McKenzie Friend", 2, 6},
                    // Case role: Applicant
                    {19, "McKenzie Friend", 3, 6},
                    // Case role: Respondent
                    {20, "McKenzie Friend", 4, 6}
                });
        }
    }
}
