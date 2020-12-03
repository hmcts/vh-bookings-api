using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class Adding_ImmigrationAndAsylum_Casetype_roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 19, "Immigration and Asylum" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 75, "Appeals - Substantive Statutory", 19 },
                    { 76, "Bail Hearing", 19 },
                    { 77, "Payment Liability Hearing", 19 },
                    { 78, "Costs Hearing", 19 },
                    { 79, "Case Management Review", 19 },
                    { 80, "Preliminary Hearing", 19 },
                    { 81, "Case Management Appointment", 19 },
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 90, "Appellant", (int) CaseRoleGroup.Appellant, 19 },
                    { 91, "Home Office", (int) CaseRoleGroup.HomeOffice, 19 },
                    { 92, "Observer", (int) CaseRoleGroup.Observer, 19 },
                    { 93, "Panel Member", (int) CaseRoleGroup.PanelMember, 19 },
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant
                    { 214, "Litigant in person", 5, 90 },
                    { 215, "Representative", 6, 90 },
                    { 216, "Witness", 5, 90 },
                    
                    // Home office
                    { 217, "Presenting Officer", 6, 91 },
                    { 218, "Witness", 5, 91 },
                    
                    // Observer
                    { 219, "Observer", 5, 92 },
                    
                    //Panel member
                    { 220, "Panel Member", 5, 93 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var hearingRoleId = 214; hearingRoleId <= 220; hearingRoleId++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleId);
            }
            
            for (var caseRoleRoleId = 90; caseRoleRoleId <= 93; caseRoleRoleId++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", caseRoleRoleId);
            }
            
            for (var hearingTypeId = 75; hearingTypeId <= 81; hearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", hearingTypeId);
            }
            
            migrationBuilder.DeleteData("CaseType", "Id", 19);
        }
    }
}
