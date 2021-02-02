using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
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
                    { 32, "Immigration and Asylum" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 114, "Appeals - Substantive Statutory", 32 },
                    { 115, "Bail Hearing", 32 },
                    { 116, "Payment Liability Hearing", 32 },
                    { 117, "Costs Hearing", 32 },
                    { 118, "Case Management Review", 32 },
                    { 119, "Preliminary Hearing", 32 },
                    { 120, "Case Management Appointment", 32 },
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 155, "Appellant", (int) CaseRoleGroup.Appellant, 32 },
                    { 156, "Home Office", (int) CaseRoleGroup.HomeOffice, 32 },
                    { 157, "Observer", (int) CaseRoleGroup.Observer, 32 },
                    { 158, "Panel Member", (int) CaseRoleGroup.PanelMember, 32 },
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant
                    { 331, "Litigant in person", 5, 155 },
                    { 332, "Representative", 6, 155 },
                    { 333, "Witness", 5, 155 },
                    
                    // Home office
                    { 334, "Presenting Officer", 6, 156 },
                    { 335, "Witness", 5, 156 },
                    
                    // Observer
                    { 336, "Observer", 5, 157 },
                    
                    //Panel member
                    { 337, "Panel Member", 5, 158 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var hearingRoleId = 331; hearingRoleId <= 337; hearingRoleId++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleId);
            }
            
            for (var caseRoleRoleId = 155; caseRoleRoleId <= 158; caseRoleRoleId++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", caseRoleRoleId);
            }
            
            for (var hearingTypeId = 114; hearingTypeId <= 120; hearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", hearingTypeId);
            }
            
            migrationBuilder.DeleteData("CaseType", "Id", 32);
        }
    }
}
