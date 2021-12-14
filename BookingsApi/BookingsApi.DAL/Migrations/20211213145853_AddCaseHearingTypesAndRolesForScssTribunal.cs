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
                    {769, HearingRoles.FamilyMember, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {770, HearingRoles.Friend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {771, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {772, HearingRoles.JointParty, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {773, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 255},
                    {774, HearingRoles.SupportWorker, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    {775, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 255},
                    
                    // Panel Member(257)
                    {784, HearingRoles.DisabilityMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 256},
                    {785, HearingRoles.FinancialMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 256},
                    {786, HearingRoles.LegalMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 256},
                    {787, HearingRoles.MedicalMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 256},
                    
                    // Respondent(256)
                    {776, HearingRoles.App, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257},
                    {777, HearingRoles.AppAdvocate, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 257},
                    {778, HearingRoles.DwpPresentingOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257},
                    {779, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257},
                    {780, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257},
                    {781, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 257},
                    {782, HearingRoles.Respondent, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257},
                    {783, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257},
                    
                    
                    // Observer(258)
                    {788, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 258},
                    {789, HearingRoles.Appraiser, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 258},
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            for (int hearingTypeId = 179; hearingTypeId <= 185; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            for (int hearingRoleId = 769; hearingRoleId <= 789; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }
            
            for (int CaseRoleId = 255; CaseRoleId <= 258; CaseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", CaseRoleId);
            }
            
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 40);


        }
    }
}
