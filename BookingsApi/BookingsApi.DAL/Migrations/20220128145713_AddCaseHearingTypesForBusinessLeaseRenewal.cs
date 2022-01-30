using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseHearingTypesForBusinessLeaseRenewal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {42, "Business Lease Renewal"}
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {214, "Case Management ", 42},
                    {215, "Mediation", 42},
                    {216, "Preliminary Hearing", 42},
                    {217, "Substantive Hearing", 42},
                    {218, "Hearing", 42}
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {                    
                    { 267, "Applicant", (int) CaseRoleGroup.Applicant, 42 },
                    { 268, "Respondent", (int) CaseRoleGroup.Respondent, 42 },
                    { 269, "Panel Member", (int) CaseRoleGroup.PanelMember, 42 },
                    { 270, "Observer", (int) CaseRoleGroup.Observer, 42 },
                    { 271, "Judge", (int) CaseRoleGroup.Judge, 42 }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Applicant(267)
                    {826, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 267},
                    {827, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 267},
                    {828, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 267},
                    {829, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 267},
                    
                    // Respondent(268)
                    {830, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 268},
                    {831, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 268},
                    {832, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 268},
                    {833, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 268},
                    
                    // Panel Member(269)
                    {834, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 269},
                    // Observer(270)
                    {835, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 270},
                    //Judge(271)
                    {836, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 271},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingTypeId = 214; hearingTypeId <= 218; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            for (int hearingRoleId = 826; hearingRoleId <= 836; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }
            
            for (int caseRoleId = 267; caseRoleId <= 271; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 42);
        }
    }
}
