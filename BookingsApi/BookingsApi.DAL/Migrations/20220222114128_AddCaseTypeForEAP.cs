using BookingsApi.Contract.Enums;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeForEAP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                          nameof(CaseType),
                          new[] { "Id", "Name" },
                          new object[,]
                          {
                              {47, "Employment Appeal Tribunal"}
                          });

            migrationBuilder.InsertData(
               nameof(HearingType),
               new[] { "Id", "Name", "CaseTypeId" },
               new object[,]
               {
                    {244, "Appeal from Registrar's Direction", 47},
                    {245, "Appeal from Registrar's Order", 47},
                    {246, "Appeal from Reinstatement Committee", 47},
                    {247, "Application for Extension of Time", 47},
                    {248, "Application to Strike Out", 47},
                    {249, "Appointment for Directions", 47},
                    {250, "Application for Costs", 47},
                    {251, "Disposal Hearings", 47},
                    {252, "For Mention", 47},
                    {253, "Interim Hearing", 47},
                    {254, "Oral Judgment", 47},
                    {255, "Preliminary Hearing (All Parties)", 47},
                    {256, "Preliminary Hearing (Appellant Only)", 47},
                    {257, "Preliminary Hearing (Respondent Only)", 47},
                    {258, "Review Application", 47},
                    {259, "Rule 3(10) Application", 47},
                    {260, "Rule 6(16) Application", 47},
               });

            migrationBuilder.InsertData(
               nameof(CaseRole),
               new[] { "Id", "Name", "Group", "CaseTypeId" },
               new object[,]
               {
                    { 294, "Apellant", (int) CaseRoleGroup.Appellant, 47 },
                    { 295, "Respondent", (int) CaseRoleGroup.Respondent, 47 },
                    { 296, "Observer", (int) CaseRoleGroup.Observer, 47 },
                    { 297, "Panel Member", (int) CaseRoleGroup.PanelMember, 47 },
                    { 298, "ELAAS", (int) CaseRoleGroup.ELAAS, 47 },
               });


            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant(294)
                    {913, HearingRoles.Advocate, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 294},
                    {914, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 294},
                    {915, HearingRoles.CertificationOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {916, HearingRoles.Counsel, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 294},
                    {917, HearingRoles.Employer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {918, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {919, HearingRoles.Interventer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {920, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {921, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {922, HearingRoles.LocalAuthority, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {923, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 294},
                    {924, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 294},
                    {925, HearingRoles.TradeUnion, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},
                    {926, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 294},

                    // Respondent(295)
                    {927, HearingRoles.Advocate, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 295},
                    {928, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 295},
                    {929, HearingRoles.CertificationOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {930, HearingRoles.Counsel, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 295},
                    {931, HearingRoles.Employer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {932, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {933, HearingRoles.Interventer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {934, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {935, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {936, HearingRoles.LocalAuthority, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {937, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 295},
                    {938, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 295},
                    {939, HearingRoles.TradeUnion, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},
                    {940, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 295},

                    // Observer(296)
                    {941, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 296},

                    // Panel Member(297)
                    {942, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 297},

                    // Secretary of State(298)
                    {943, HearingRoles.ELAAS, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 298},
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 913; hearingRoleId <= 943; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 294; caseRoleId <= 298; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 244; hearingTypeId <= 260; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 47);
        }
    }
}
