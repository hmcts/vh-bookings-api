using System;
using System.Globalization;
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
               new[] { "Id", "Name", "Group", "CaseTypeId", "CreatedDate" },
               new object[,]
               {
                    { 343, "Applicant", (int) CaseRoleGroup.Applicant, 52, DateTime.UtcNow },
                    { 344, "Respondent", (int) CaseRoleGroup.Respondent, 52, DateTime.UtcNow },
               });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "CreatedDate" },
                new object[,]
                {
                    //Applicants
                    { 1094, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1095, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1096, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1097, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1098, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1099, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1100, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    { 1101, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 343, DateTime.UtcNow },
                    { 1102, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 343, DateTime.UtcNow },
                    { 1103, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 343, DateTime.UtcNow },
                    
                    //Respondent
                    { 1104, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1105, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1106, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1107, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1108, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1109, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1110, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
                    { 1111, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 344, DateTime.UtcNow },
                    { 1112, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 344, DateTime.UtcNow },
                    { 1113, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 344, DateTime.UtcNow },
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int caseRoleId = 1094; caseRoleId <= 1113; caseRoleId++)
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", caseRoleId);

            migrationBuilder.DeleteData(nameof(CaseRole), "Id", 344);
            migrationBuilder.DeleteData(nameof(CaseRole), "Id", 343);
        }
    }
}
