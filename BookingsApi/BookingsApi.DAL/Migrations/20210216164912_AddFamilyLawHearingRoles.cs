using BookingsApi.Contract.V1.Enums;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddFamilyLawHearingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 136, "Application", 6 },
                    { 137, "Case Management Conference", 6 },
                    { 138, "Directions", 6 },
                    { 139, "First hearing", 6 },
                    { 140, "Full", 6 },
                });
            
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {175, "Appellant", (int) CaseRoleGroup.Appellant, 6},
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // appellant (175)
                    { 488, "Barrister", 6, 175 },
                    { 489, "Expert", 5, 175 },
                    { 490, "Intermediary", 5, 175 },
                    { 491, "Interpreter", 5, 175 },
                    { 492, "Litigant in person", 5, 175 },
                    { 493, "Litigation friend", 5, 175 },
                    { 494, "MacKenzie friend", 5, 175 },
                    { 495, "Representative", 6, 175 },
                    { 496, "Solicitor", 6, 175 },
                    { 497, "Witness", 5, 175 },
                    // applicant (16)
                    { 498, "Barrister", 6, 16 },
                    { 499, "Expert", 5, 16 },
                    { 500, "Intermediary", 5, 16 },
                    { 501, "Litigation friend", 5, 16 },
                    { 502, "MacKenzie friend", 5, 16 },
                    { 503, "Solicitor", 6, 16 },
                    // respondent (17)
                    { 504, "Barrister", 6, 17 },
                    { 505, "Expert", 5, 17 },
                    { 506, "Intermediary", 5, 17 },
                    { 507, "Litigation friend", 5, 17 },
                    { 508, "MacKenzie friend", 5, 17 },
                    { 509, "Solicitor", 6, 17 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var i = 136; i < 141; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }
            
            migrationBuilder.DeleteData("CaseRole", "Id", 175);
            
            for (var i = 488; i < 510; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
