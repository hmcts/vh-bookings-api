using BookingsApi.Contract.Enums;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseHearingTypesAndRolesForScssTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 40, "SSCS Tribunal" }
                });
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 179, "01 Appeals", 40 },
                    { 180, "02 Appeals", 40 },
                    { 181, "03 Appeals", 40 },
                    { 182, "04 Appeals", 40 },
                    { 183, "05/06/07 Appeals", 40 },
                    { 184, "Pre Hearing Reviews", 40 },
                    { 185, "Liberty to Apply Hearings", 40 }
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 255, "Appellant", (int) CaseRoleGroup.Appellant, 40 },
                    { 256, "Panel Member", (int) CaseRoleGroup.PanelMember, 40 },
                    { 257, "None", (int) CaseRoleGroup.None, 40 },
                    { 258, "Observer", (int) CaseRoleGroup.Observer, 40 }
                });
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant(255)
                    {769, nameof(HearingRoles.FamilyMember), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {770, nameof(HearingRoles.Friend), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {771, nameof(HearingRoles.Interpreter), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {772, nameof(HearingRoles.JointParty), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {773, nameof(HearingRoles.Representative), UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 255},
                    {774, nameof(HearingRoles.SupportWorker), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {775, nameof(HearingRoles.Witness), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    // Respondent(256)
                    {776, nameof(HearingRoles.App), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 256},
                    {777, nameof(HearingRoles.AppAdvocate), UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 256},
                    {778, nameof(HearingRoles.DwpPresentingOfficer), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 256},
                    {779, nameof(HearingRoles.Expert), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 256},
                    {780, nameof(HearingRoles.Interpreter), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 256},
                    {781, nameof(HearingRoles.Representative), UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 256},
                    {782, nameof(HearingRoles.Respondent), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 256},
                    {783, nameof(HearingRoles.Witness), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 256},
                    
                    // Panel Member(257)
                    {784, nameof(HearingRoles.DisabilityMember), UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 257},
                    {785, nameof(HearingRoles.FinancialMember), UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 257},
                    {786, nameof(HearingRoles.LegalMember), UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 257},
                    {787, nameof(HearingRoles.MedicalMember), UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 257},
                    // bserver(258)
                    {788, nameof(HearingRoles.Observer), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 258},
                    {789, nameof(HearingRoles.Appraiser), UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 258},
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 40);

            for (int hearingTypeId = 179; hearingTypeId <= 185; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }
            
            for (int CaseRoleId = 255; CaseRoleId <= 258; CaseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", CaseRoleId);
            }

            for (int hearingRoleId = 769; hearingRoleId <= 788; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

        }
    }
}
