using BookingsApi.Contract.Enums;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeAsylumSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              nameof(CaseType),
              new[] { "Id", "Name" },
              new object[,]
              {
                    { 48, "Asylum Support" }
              });

            migrationBuilder.InsertData(
               nameof(HearingType),
               new[] { "Id", "Name", "CaseTypeId" },
               new object[,]
               {
                    { 261, "Case Management Review Hearing", 48 },
                    { 262, "Adjourned/Resumed Hearing", 48 },
                    { 263, "Full Hearing", 48 },
                    { 264, "Hearing to determine possible strike out/validity", 48 },
                    { 265, "Hearing do novo - following an internal set aside of a previous decision", 48 },
                    { 266, "Hearing do novo - following remittal by the High Court", 48 }
               });

            migrationBuilder.InsertData(
               nameof(CaseRole),
               new[] { "Id", "Name", "Group", "CaseTypeId" },
               new object[,]
               {
                    { 301, "Appellant", (int) CaseRoleGroup.Appellant, 48 },
                    { 302, "Applicant", (int) CaseRoleGroup.Applicant, 48 },
                    { 303, "Observer", (int) CaseRoleGroup.Observer, 48 },
                    { 304, "Panel Member", (int) CaseRoleGroup.PanelMember, 48 },
                    { 305, "Secretary of State", (int) CaseRoleGroup.SecretaryOfState, 48 },
                    { 306, "Home Office", (int) CaseRoleGroup.HomeOffice, 48 },
                    { 307, "Judge", (int) CaseRoleGroup.Judge, 48 }
               });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant (301)
                    { 946, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 301 },
                    { 947, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 301 },
                    { 948, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 301 },
                    { 949, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 301 },
                    { 950, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 301 },
                    { 951, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 301 },
                    { 952, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 301 },
                    { 953, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 301 },
                     
                    // Applicant (302)
                    { 954, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 302 },
                    { 955, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 302 },
                    { 956, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 302 },
                    { 957, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 302 },
                    { 958, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 302 },
                    { 959, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 302 },
                    { 960, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 302 },
                    { 961, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 302 },

                    // Observer (303)
                    { 962, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 303 },

                    // Panel Member (304)
                    { 963, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 304 },

                    // Secretary of State (305)
                    { 964, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 305 },
                    { 965, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 305 },
                    { 966, HearingRoles.GovernmentLegalDepartmentSolicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 305 },

                    // Home Office (306)
                    { 967, HearingRoles.PresentingOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 306 },
                    { 968, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 306 },

                     // Judge (307)
                    { 969, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 307 }
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 946; hearingRoleId <= 969; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 301; caseRoleId <= 307; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 261; hearingTypeId <= 266; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 48);
        }
    }
}
