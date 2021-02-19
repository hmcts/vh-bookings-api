using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseHearingTypesAndRolesForPublicLawCare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 35, "Public Law - Care" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 162, "Application", 35 },
                    { 163, "Case Management Conference", 35 },
                    { 164, "Case Management Hearing", 35 },
                    { 165, "Directions", 35 },
                    { 166, "Full", 35 },
                    { 167, "Further CMH", 35 },
                    { 168, "Interim Care Order", 35 },
                    { 169, "Issues Resolution Hearing", 35 },
                    { 170, "Pre Hearing Review", 35 }
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 186, "Appellant", (int) CaseRoleGroup.Appellant, 35 },
                    { 187, "Applicant", (int) CaseRoleGroup.Applicant, 35 },
                    { 188, "Respondent", (int) CaseRoleGroup.Respondent, 35 },
                    { 189, "Observer", (int) CaseRoleGroup.Observer, 35 }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId" },
                new object[,]
                {
                    { 604, "Barrister", 6, 186 },
                    { 605, "Expert", 5, 186 },
                    { 606, "Intermediary", 5, 186 },
                    { 607, "Interpreter", 5, 186 },
                    { 608, "Litigant in person", 5, 186 },
                    { 609, "Litigation friend", 5, 186 },
                    { 610, "MacKenzie friend", 5, 186 },
                    { 611, "Representative", 6, 186 },
                    { 612, "Solicitor", 6, 186 },
                    { 613, "Witness", 5, 186 },

                    { 614, "Barrister", 6, 187 },
                    { 615, "Expert", 5, 187 },
                    { 616, "Intermediary", 5, 187 },
                    { 617, "Interpreter", 5, 187 },
                    { 618, "Litigant in person", 5, 187 },
                    { 619, "Litigation friend", 5, 187 },
                    { 620, "MacKenzie friend", 5, 187 },
                    { 621, "Representative", 6, 187 },
                    { 622, "Solicitor", 6, 187 },
                    { 623, "Witness", 5, 187 },
                   
                    { 624, "Barrister", 6, 188 },
                    { 625, "Expert", 5, 188 },
                    { 626, "Intermediary", 5, 188 },
                    { 627, "Interpreter", 5, 188 },
                    { 628, "Litigant in person", 5, 188 },
                    { 629, "Litigation friend", 5, 188 },
                    { 630, "MacKenzie friend", 5, 188 },
                    { 631, "Representative", 6, 188 },
                    { 632, "Solicitor", 6, 188 },
                    { 633, "Witness", 5, 188 },
                    
                    { 634, "Observer", 5, 189 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("CaseType", "Id", 35);

            for (var i = 162; i < 171; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }

            for (var i = 186; i < 190; i++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", i);
            }

            for (var i = 604; i < 635; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
