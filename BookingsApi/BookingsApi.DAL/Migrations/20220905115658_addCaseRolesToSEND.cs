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
                    { 1081, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1082, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1083, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1084, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1085, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1086, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1087, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    { 1088, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 343 },
                    { 1089, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 343 },
                    { 1090, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343 },
                    
                    //Respondent
                    { 1091, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1092, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1093, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1094, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1095, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1096, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1097, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
                    { 1098, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 344 },
                    { 1099, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 344 },
                    { 1100, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344 },
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
