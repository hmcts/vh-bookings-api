
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeCriminalInjusticeCompensation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              nameof(CaseType),
              new[] { "Id", "Name" },
              new object[,]
              {
                    { 49, "Criminal Injuries Compensation" }
              });

            migrationBuilder.InsertData(
               nameof(HearingType),
               new[] { "Id", "Name", "CaseTypeId" },
               new object[,]
               {
                    { 269, "Case Management Discussion", 49 },
                    { 270, "Case Management Hearing", 49 },
                    { 271, "Oral Hearing", 49 },
               });

            migrationBuilder.InsertData(
               nameof(CaseRole),
               new[] { "Id", "Name", "Group", "CaseTypeId" },
               new object[,]
               {
                    { 308, "Appellant", (int) CaseRoleGroup.Appellant, 49 },
                    { 309, "Observer", (int) CaseRoleGroup.Observer, 49 },
                    { 310, "None", (int) CaseRoleGroup.None, 49 },
                    { 311, "Panel Member", (int) CaseRoleGroup.PanelMember, 49 },
                    { 312, "Presenting Officer", (int) CaseRoleGroup.PresentingOfficer, 49 },
                    { 313, "Judge", (int) CaseRoleGroup.Judge, 49 },
               });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant (308)
                    { 970, HearingRoles.Counsel, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 308 },
                    { 971, HearingRoles.FamilyMember, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 308 },
                    { 972, HearingRoles.Friend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 308 },
                    { 973, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 308 },
                    { 974, HearingRoles.LegalRepresentative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 308 },
                    { 975, HearingRoles.PoliceOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 308 },
                    { 976, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 308 },
                    { 977, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 308 },
                    { 978, HearingRoles.SupportWorker, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 308 },
                    { 979, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 308 },
                     
                    // Observer (309)
                    { 980, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 309 },
                    { 981, HearingRoles.Appraiser, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 309 },

                    // None (310)
                    { 982, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 310 },
                    { 983, HearingRoles.AppellantAdvocate, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 310 },
                    { 984, HearingRoles.Expert, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 310 },
                    { 985, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 310 },
                    { 986, HearingRoles.Respondent, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 310 },
                    { 987, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 310 },

                    // Panel Member (311)
                    { 988, HearingRoles.LayMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 311 },
                    { 989, HearingRoles.MedicalMember, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 311 },

                    // Presenting Officer(312)
                    { 990, HearingRoles.CICAPresentingOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 312 },
                    { 991, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 312 },

                     // Judge (313)
                    { 992, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 313 }
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 970; hearingRoleId <= 992; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 308; caseRoleId <= 313; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 269; hearingTypeId <= 271; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 49);
        }
    }
}
