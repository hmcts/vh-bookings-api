using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Helper;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddedNewCaseTypeCareStandards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name", "CreatedDate" },
                new object[,]
                {
                    { 53, "Care Standards", DateTime.UtcNow }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId", "CreatedDate" },
                new object[,]
                {
                    { 291, "Case Management Hearing", 53, DateTime.UtcNow },
                    { 292, "Final Hearing", 53, DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId", "CreatedDate" },
                new object[,]
                {
                    { 345, "Appellant", (int) CaseRoleGroup.Appellant, 53, DateTime.UtcNow },
                    { 346, "Local Authority", (int) CaseRoleGroup.LocalAuthority, 53, DateTime.UtcNow },
                    { 347, "Observer", (int) CaseRoleGroup.Observer, 53, DateTime.UtcNow },
                    { 348, "Panel Member", (int) CaseRoleGroup.PanelMember, 53, DateTime.UtcNow },
                    { 349, "Judge", (int) CaseRoleGroup.Judge, 53, DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "CreatedDate" },
                new object[,]
                {
                    // Appellant 345
                    { 1114, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1115, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1116, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1117, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1118, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1119, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1120, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    { 1121, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 345 , DateTime.UtcNow },
                    { 1122, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 345 , DateTime.UtcNow },
                    { 1123, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 345 , DateTime.UtcNow },
                    // Local Authortity 346
                    { 1124, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 346 , DateTime.UtcNow },
                    { 1125, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 346 , DateTime.UtcNow },
                    // Observer 337
                    { 1126, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 347 , DateTime.UtcNow },
                    // Panel Member 338
                    { 1127, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 348 , DateTime.UtcNow },
                    // Judge 339
                    { 1128, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 349 , DateTime.UtcNow },
                    
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 1114; hearingRoleId <= 1129; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 345; caseRoleId <= 349; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            migrationBuilder.DeleteData(nameof(HearingType), "Id", 292);
            migrationBuilder.DeleteData(nameof(HearingType), "Id", 291);
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 53);
        }
    }
}
