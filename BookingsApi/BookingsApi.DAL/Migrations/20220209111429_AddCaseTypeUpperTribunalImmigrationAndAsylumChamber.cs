using BookingsApi.Domain.Enumerations;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeUpperTribunalImmigrationAndAsylumChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {45, "Upper Tribunal Immigration & Asylum Chamber"}
                });


            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {230, "Substantive", 45},
                    {231, "Oral Permission/Renewal", 45},
                    {232, "Case Management Review", 45},
                    {233, "Error of Law", 45},
                    {234, "For Mention", 45},
                    {235, "Hamid", 45},
                    {236, "Rolled up", 45},
                    {237, "Oral Permission", 45 },
                    {238, "Resumed/Continuance", 45},
                    {239, "Hand down", 45},
                    {240, "Urgent Oral", 45},
                });


            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 281, "Appellant", (int) CaseRoleGroup.Appellant, 45 },
                    { 282, "Applicant", (int) CaseRoleGroup.Applicant, 45 },
                    { 283, "Home Office", (int) CaseRoleGroup.HomeOffice, 45 },
                    { 284, "Observer", (int) CaseRoleGroup.Observer, 45 },
                    { 285, "Judge", (int) CaseRoleGroup.PanelMember, 45 },
                    { 286, "Panel Member", (int) CaseRoleGroup.Observer, 45 },
                    { 287, "Secretary of State", (int) CaseRoleGroup.SecretaryOfState, 45 },

                });


            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    //// Appellant(281)
                    {866, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 281},
                    {867, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},
                    {868, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},
                    {869, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},
                    {870, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},
                    {871, HearingRoles.LocalAuthority, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},
                    {872, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},
                    {873, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 281},
                    {874, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 281},
                    {875, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 281},

                    
                    //// Applicant(282)
                    {876, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 282},
                    {877, HearingRoles.Intermediary, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    {878, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    {879, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    {880, HearingRoles.LitigationFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    {881, HearingRoles.LocalAuthority, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    {882, HearingRoles.MacKenzieFriend, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    {883, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 282},
                    {884, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 282},
                    {885, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 282},
                    
                    // Home Office(283)
                    {886, HearingRoles.PresentingOfficer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 283},
                    {887, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 283},

                    // Observer(284)
                    {888, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 284},
                    
                    //Judge(285)
                    {889, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 285},

                    // Panel Member(286)
                    {890, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 286},

                     // Secretary of State(287)
                    {891, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 287},
                    {892, HearingRoles.GovernmentLegalDepartmentSolicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 287},

                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 866; hearingRoleId <= 892; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 281; caseRoleId <= 287; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 230; hearingTypeId <= 240; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }
            
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 45);
        }
    }
}
