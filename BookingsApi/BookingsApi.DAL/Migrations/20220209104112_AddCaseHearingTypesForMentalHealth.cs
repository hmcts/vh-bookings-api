using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseHearingTypesForMentalHealth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {44, "Mental Health"}
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {223, "Non-Restricted Application", 44},
                    {224, "Non-Restricted Referral", 44},
                    {225, "Restricted Application", 44},
                    {226, "Restricted Referral", 44},
                    {227, "Community", 44},
                    {228, "Hearing", 44},
                    {229, "Section 2", 44},
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 277, "Applicant", (int) CaseRoleGroup.Applicant, 44 },
                    { 278, "Panel Member", (int) CaseRoleGroup.PanelMember, 44 },
                    { 279, "Observer", (int) CaseRoleGroup.Observer, 44 },
                    { 280, "Judge", (int) CaseRoleGroup.Judge, 44 }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Applicant(277)
                    {848, HearingRoles.CareCoordinator, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {849, HearingRoles.Doctor, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {850, HearingRoles.FamilyMember, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {851, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {852, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {853, HearingRoles.NearestRelative, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {854, HearingRoles.Nurse, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {855, HearingRoles.Patient, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {856, HearingRoles.ResponsibleClinician, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {857, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {858, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {859, HearingRoles.VictimLiaisonOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    {860, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 277},
                    // Panel Member(278)
                    {861, HearingRoles.SpecialistLayMember, UserRoleForHearingRole.UserRoleId[UserRoles.PanelMember], 278},
                    {862, HearingRoles.MedicalMember, UserRoleForHearingRole.UserRoleId[UserRoles.PanelMember], 278},
                    // Observer(279)
                    {863, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Observer], 279},
                    {864, HearingRoles.Appraiser, UserRoleForHearingRole.UserRoleId[UserRoles.Observer], 279},
                    // Judge(280)
                    {865, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 280}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 848; hearingRoleId <= 865; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 277; caseRoleId <= 280; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }
            
            for (int hearingTypeId = 223; hearingTypeId <= 229; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 44);
        }
    }
}
