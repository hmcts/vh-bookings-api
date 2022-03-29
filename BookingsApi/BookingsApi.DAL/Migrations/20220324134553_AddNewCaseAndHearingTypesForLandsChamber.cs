using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewCaseAndHearingTypesForLandsChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 51, "Upper Tribunal Lands Chamber" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 274, "Substantive", 51 },
                    { 275, "Case Management Hearing", 51 },
                    { 276, "Preliminary Hearing", 51 },
                    { 277, "Interim Rights Hearing", 51 },
                    { 278, "Detailed Costs Assessment", 51 },
                    { 279, "For Mention", 51 },
                    { 280, "Application for permission to appeal", 51 },
                    { 281, "Simplified", 51 },
                    { 282, "First Hearing", 51 },
                    { 283, "Section 84(3A)", 51 },
                    { 284, "Interloculatory/Directions", 51 },
                    { 285, "Closing Submissions", 51 },
                    { 286, "Representation for Costs", 51 },
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 320, "Applicant", (int) CaseRoleGroup.Applicant, 51 },
                    { 321, "Appellant", (int) CaseRoleGroup.Appellant, 51 },
                    { 322, "Claimant", (int) CaseRoleGroup.Claimant, 51 },
                    { 323, "Respondent", (int) CaseRoleGroup.Respondent, 51 },
                    { 324, "Acquiring Authority", (int) CaseRoleGroup.AcquiringAuthority, 51 },
                    { 325, "Compensating Authority", (int) CaseRoleGroup.CompensatingAuthority, 51 },
                    { 326, "Objector", (int) CaseRoleGroup.Objector, 51 },
                    { 327, "Operator", (int) CaseRoleGroup.Operator, 51 },
                    { 328, "Site Providers", (int) CaseRoleGroup.SiteProviders, 51 },
                    { 329, "Respondent Authority", (int) CaseRoleGroup.RespondentAuthority, 51 },
                    { 330, "Panel Member", (int) CaseRoleGroup.PanelMember, 51 },
                    { 331, "Observer", (int) CaseRoleGroup.Observer , 51 },
                    { 332, "Judge", (int) CaseRoleGroup.Judge , 51 },
                });
            
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Applicant (320)
                    { 1013, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 320 },
                    { 1014, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 320 },
                    { 1015, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 320 },
                    { 1016, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 320 },
                    { 1017, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 320 },
                    
                    // Appellant (321)
                    { 1018, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 321 },
                    { 1019, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 321 },
                    { 1020, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 321 },
                    { 1021, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 321 },
                    { 1022, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 321 },
                    
                    // Claimant (322)
                    { 1023, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 322 },
                    { 1024, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 322 },
                    { 1025, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 322 },
                    { 1026, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 322 },
                    { 1027, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 322 },
                    
                    // Respondent (323)
                    { 1028, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 323 },
                    { 1029, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 323 },
                    { 1030, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 323 },
                    { 1031, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 323 },
                    { 1032, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 323 },
                    
                    // Acquiring Authority (324)
                    { 1033, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 324 },
                    { 1034, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 324 },
                    { 1035, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 324 },
                    { 1036, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 324 },
                    { 1037, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 324 },
                    
                    // Compensating Authority (325)
                    { 1038, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 325 },
                    { 1039, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 325 },
                    { 1040, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 325 },
                    { 1041, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 325 },
                    { 1042, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 325 },
                    
                    // Objector (326)
                    { 1043, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 326 },
                    { 1044, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 326 },
                    { 1045, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 326 },
                    { 1046, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 326 },
                    { 1047, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 326 },
                    
                    // Operator (327)
                    { 1048, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 327 },
                    { 1049, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 327 },
                    { 1050, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 327 },
                    { 1051, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 327 },
                    { 1052, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 327 },
                    
                    // Site Providers (328)
                    { 1053, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 328 },
                    { 1054, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 328 },
                    { 1055, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 328 },
                    { 1056, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 328 },
                    { 1057, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 328 },

                    // Respondent Authority (329)
                    { 1058, HearingRoles.Barrister, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 329 },
                    { 1059, HearingRoles.ExpertWitness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 329 },
                    { 1060, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 329 },
                    { 1061, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 329 },
                    { 1062, HearingRoles.Solicitor, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 329 },
                    
                    // Panel Member (330)
                    { 1063, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.JudicialOfficeHolder], 330 },
                    
                    // Observer (331)
                    { 1064, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 331 },
                    
                    // Judge (332)
                    { 1065, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 332 }
                });
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 1013; hearingRoleId <= 1065; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            for (int caseRoleId = 320; caseRoleId <= 332; caseRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", caseRoleId);
            }

            for (int hearingTypeId = 274; hearingTypeId <= 286; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 51);
        }
    }
}
