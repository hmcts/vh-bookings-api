using BookingsApi.Contract.Enums;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeWarPensionsAppeals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 50, "War Pensions Appeals" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 272, "Case Management Hearing", 50 },
                    { 273, "Full Hearing", 50 }
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 314, "Appellant", (int) CaseRoleGroup.Appellant, 50 },
                    { 315, "In absence", (int) CaseRoleGroup.InAbsence, 50 },
                    { 316, "Observer", (int) CaseRoleGroup.Observer, 50 },
                    { 317, "Panel Member", (int) CaseRoleGroup.PanelMember, 50 },
                    { 318, "Vets UK", (int) CaseRoleGroup.VetsUK, 50 },
                    { 319, "Judge", (int) CaseRoleGroup.Judge, 50 }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant (314)
                    { 993, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 314 },
                    { 994, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 314 },
                    { 995, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 314 },
                    { 996, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 314 },
                    { 997, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 314 },
                    { 998, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 314 },
                    { 999, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 314 },
                    { 1000, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 314 },
                    { 1001, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 314 },
                    
                    // In absence (315)
                    { 1002, HearingRoles.AppAdvocate, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 315 },
                    { 1003, HearingRoles.RoyalBritishLegion, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 315 },
                    { 1004, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 315 },
                    { 1005, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 315 },
                    { 1006, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 315 },
                    
                    // Observer (316)
                    { 1007, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 316 },
                    
                    // Panel Member (317)
                    { 1008, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 317 },
                    
                    // Vets UK (318)
                    { 1009, HearingRoles.PresentingOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 318 },
                    { 1010, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 318 },
                    { 1011, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 318 },
                    
                    // Judge (319)
                    { 1012, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 319 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 993; hearingRoleId <= 1012; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 314; caseRoleId <= 319; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 272; hearingTypeId <= 273; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 50);
        }
    }
}
