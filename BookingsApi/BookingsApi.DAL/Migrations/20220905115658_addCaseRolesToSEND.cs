using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class addCaseRolesToSEND : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               nameof(CaseRole),
               new[] { "Id", "Name", "Group", "CaseTypeId" },
               new object[,]
               {
                    { 343, "Applicant", (int) CaseRoleGroup.Applicant, 53 },
                    { 344, "Respondant", (int) CaseRoleGroup.Respondent, 53 },
               });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    //Applicants
                    { 1094, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1095, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1096, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1097, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1098, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1099, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1100, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1101, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 343 },
                    { 1102, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 343 },
                    { 1103, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    
                    //Respondent
                    { 1104, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1105, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1106, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1107, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1108, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1109, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1110, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1111, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 344 },
                    { 1112, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 344 },
                    { 1113, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
     
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 343);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 344);
            
            for (int caseRoleId = 1081; caseRoleId <= 1100; caseRoleId++)
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            
        }
    }
}
