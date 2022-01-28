using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewCaseHearingTypesAndRolesToFamilyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {41, "Family"}
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {205, "Divorce", 41},
                    {206, "Financial Remedy", 41},
                    {207, "Family Public Law", 41},
                    {208, "Adoption", 41},
                    {209, "Family Private Law", 41},
                    {210, "Probate", 41},
                    {211, "Court of Protections", 41},
                    {212, "REMO", 41},
                    {213, "Maintenance Enforcement", 41}
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 261, "Appellant", (int) CaseRoleGroup.Appellant, 41 },
                    { 262, "Applicant", (int) CaseRoleGroup.Applicant, 41 },
                    { 263, "Respondent", (int) CaseRoleGroup.Respondent, 41 },
                    { 264, "Panel Member", (int) CaseRoleGroup.PanelMember, 41 },
                    { 265, "Observer", (int) CaseRoleGroup.Observer, 41 },
                    { 266, "Judge", (int) CaseRoleGroup.Judge, 41 }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant(261)
                    {793, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 261},
                    {794, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    {795, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    {796, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    {797, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    {798, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    {799, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    {800, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 261},
                    {801, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 261},
                    {802, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 261},
                    // Applicant(262)
                    {803, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 262},
                    {804, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    {805, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    {806, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    {807, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    {808, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    {809, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    {810, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 262},
                    {811, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 262},
                    {812, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 262},
                    // Respondent(263)
                    {813, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 263},
                    {814, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    {815, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    {816, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    {817, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    {818, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    {819, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    {820, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 263},
                    {821, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 263},
                    {822, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 263},
                    // Panel Member(264)
                    {823, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.PanelMember], 264},
                    // Observer(265)
                    {824, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 265},
                    //Judge(266)
                    {825, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 266},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 41);
            
            for (int hearingTypeId = 205; hearingTypeId <= 213; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }
            
            for (int caseRoleId = 261; caseRoleId <= 266; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }
            
            for (int hearingRoleId = 793; hearingRoleId <= 825; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }
        }
    }
}
