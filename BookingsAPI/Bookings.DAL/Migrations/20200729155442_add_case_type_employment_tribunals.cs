using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class add_case_type_employment_tribunals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 17, "Employment Tribunal" }
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 51, "Public Preliminary Hearing", 17 },
                    { 52, "Private Preliminary Hearing", 17 },
                    { 53, "Mediation Hearing", 17 },
                    { 54, "Interim Relief Hearing", 17 },
                    { 55, "Substantive Hearing (Judge sit alone)", 17 },
                    { 56, "Substantive Hearing (Full Panel)", 17 },
                    { 57, "Costs Hearing", 17 },
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 83, "Claimant", (int) CaseRoleGroup.Claimant, 17 },
                    { 84, "Respondent", (int) CaseRoleGroup.Respondent, 17 },
                    { 85, "Judge", (int) CaseRoleGroup.Judge, 17 },
                    { 86, "Observer", (int) CaseRoleGroup.Observer, 17 },
                    { 87, "Panel Member", (int) CaseRoleGroup.PanelMember, 17 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 121, "Claimaint LIP", 5, 83 },
                    { 122, "Representative", 6, 83 },
                    { 123, "Respondent LIP", 5, 84 },
                    { 124, "Representative", 6, 84 },
                    { 125, "Judge", 4, 85 },
                    { 126, "Observer", 5, 86 },
                    { 127, "Panel Member", 5, 87 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 121; hearingRoleID <= 127; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }

            for (int CaseRoleRoleID = 83; CaseRoleRoleID <= 87; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }

            for (int HearingTypeId = 51; HearingTypeId <= 57; HearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", HearingTypeId);
            }

            migrationBuilder.DeleteData("CaseType", "Id", 17);
        }
    }
}
