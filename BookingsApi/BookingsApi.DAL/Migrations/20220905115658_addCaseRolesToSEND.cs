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
                    { 338, "Applicant", (int) CaseRoleGroup.Applicant, 52 },
                    { 339, "Respondant", (int) CaseRoleGroup.Respondent, 52 },
               });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    //Applicants
                    { 1081, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1082, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1083, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1084, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1085, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1086, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1087, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    { 1088, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 338 },
                    { 1089, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 338 },
                    { 1090, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 338 },
                    
                    //Respondent
                    { 1091, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1092, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1093, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1094, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1095, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1096, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1097, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
                    { 1098, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 339 },
                    { 1099, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 339 },
                    { 1100, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 339 },
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
     
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 338);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 339);
            
            for (int caseRoleId = 1081; caseRoleId <= 1100; caseRoleId++)
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            
        }
    }
}
