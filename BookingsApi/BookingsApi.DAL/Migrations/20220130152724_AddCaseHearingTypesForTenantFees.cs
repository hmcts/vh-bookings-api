using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseHearingTypesForTenantFees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {43, "Tenant Fees"}
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {219, "Case Management", 43},
                    {220, "Mediation", 43},
                    {221, "Preliminary Hearing", 43},
                    {222, "Substantive Hearing", 43},
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 272, "Applicant", (int) CaseRoleGroup.Applicant, 43 },
                    { 273, "Respondent", (int) CaseRoleGroup.Respondent, 43 },
                    { 274, "Panel Member", (int) CaseRoleGroup.PanelMember, 43 },
                    { 275, "Observer", (int) CaseRoleGroup.Observer, 43 },
                    { 276, "Judge", (int) CaseRoleGroup.Judge, 43 }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Applicant(272)
                    {837, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 272},
                    {838, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 272},
                    {839, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 272},
                    {840, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 272},
                    // Respondent(273)
                    {841, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 273},
                    {842, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 273},
                    {843, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 273},
                    {844, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 273},
                    // Panel Member(274)
                    {845, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.PanelMember], 274},
                    // Observer(275)
                    {846, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 275},
                    //Judge(276)
                    {847, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 276},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingTypeId = 219; hearingTypeId <= 222; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            for (int hearingRoleId = 837; hearingRoleId <= 847; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 272; caseRoleId <= 276; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 43);        }
    }
}
