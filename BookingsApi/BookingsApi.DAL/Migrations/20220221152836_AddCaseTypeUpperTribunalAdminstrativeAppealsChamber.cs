using BookingsApi.Contract.Enums;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeUpperTribunalAdminstrativeAppealsChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               nameof(CaseType),
               new[] { "Id", "Name" },
               new object[,]
               {
                    {46, "Upper Tribunal Administrative Appeals Chamber"}
               });

            migrationBuilder.InsertData(
               nameof(HearingType),
               new[] { "Id", "Name", "CaseTypeId" },
               new object[,]
               {
                    {241, "Case Management Review", 46},
                    {242, "Oral Permission/Renewal", 46},
                    {243, "Substantive", 46},
               });

            migrationBuilder.InsertData(
               nameof(CaseRole),
               new[] { "Id", "Name", "Group", "CaseTypeId" },
               new object[,]
               {
                    { 288, "Appellant", (int) CaseRoleGroup.Appellant, 46 },
                    { 289, "Applicant", (int) CaseRoleGroup.Applicant, 46 },
                    { 290, "Observer", (int) CaseRoleGroup.Observer, 46 },
                    { 291, "Panel Member", (int) CaseRoleGroup.PanelMember, 46 },
                    { 292, "Secretary of State", (int) CaseRoleGroup.SecretaryOfState, 46 },
                    { 293, "Traffic Commissioner", (int) CaseRoleGroup.TrafficCommissioner, 46 },
               });


            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant(288)
                    {893, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 288},
                    {894, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 288},
                    {895, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 288},
                    {896, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 288},
                    {897, HearingRoles.LocalAuthority, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 288},
                    {898, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 288},
                    {899, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 288},
                    {900, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 288},
                     
                    // Applicant(289)
                    {901, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 289},
                    {902, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 289},
                    {903, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 289},
                    {904, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 289},
                    {905, HearingRoles.LocalAuthority, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 289},
                    {906, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 289},
                    {907, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 289},
                    {908, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 289},

                    // Observer(290)
                    {909, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 290},

                    // Panel Member(291)
                    {910, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 291},

                    // Secretary of State(292)
                    {911, HearingRoles.GovernmentLegalDepartmentSolicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 292},

                    // Traffic Commissioner(293)
                    {912, HearingRoles.TrafficCommissioner, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 293},
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 893; hearingRoleId <= 912; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 288; caseRoleId <= 293; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 241; hearingTypeId <= 243; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 46);

        }
    }
}
