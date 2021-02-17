using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddPrivateLawHearingTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseType),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 33, "Private Law" },
                });
            
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 147, "Application", 33 },
                    { 148, "Case Management Conference", 33 },
                    { 149, "Directions", 33 },
                    { 150, "First hearing", 33 },
                    { 151, "Full hearing", 33 },
                    { 152, "Pre hearing review", 33 },
                    { 153, "Review", 33 },
                });

            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] {"Id", "Name", "Group", "CaseTypeId"},
                values: new object[,]
                {
                    {178, "Appellant", (int) CaseRoleGroup.Appellant, 33},
                    {179, "Applicant", (int) CaseRoleGroup.Applicant, 33},
                    {180, "Respondent", (int) CaseRoleGroup.Respondent, 33},
                    {181, "Observer", (int) CaseRoleGroup.Observer, 33},
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // appellant (178)
                    { 542, "Barrister", 6, 178 },
                    { 543, "Expert", 5, 178 },
                    { 544, "Intermediary", 5, 178 },
                    { 545, "Interpreter", 5, 178 },
                    { 546, "Litigant in person", 5, 178 },
                    { 547, "Litigation friend", 5, 178 },
                    { 548, "MacKenzie friend", 5, 178 },
                    { 549, "Representative", 6, 178 },
                    { 550, "Solicitor", 6, 178 },
                    { 551, "Witness", 5, 178 },
                    // applicant (179)
                    { 552, "Barrister", 6, 179 },
                    { 553, "Expert", 5, 179 },
                    { 554, "Intermediary", 5, 179 },
                    { 555, "Interpreter", 5, 179 },
                    { 556, "Litigant in person", 5, 179 },
                    { 557, "Litigation friend", 5, 179 },
                    { 558, "MacKenzie friend", 5, 179 },
                    { 559, "Representative", 6, 179 },
                    { 560, "Solicitor", 6, 179 },
                    { 561, "Witness", 5, 179 },
                    // respondent (180)
                    { 562, "Barrister", 6, 180 },
                    { 563, "Expert", 5, 180 },
                    { 564, "Intermediary", 5, 180 },
                    { 565, "Interpreter", 5, 180 },
                    { 566, "Litigant in person", 5, 180 },
                    { 567, "Litigation friend", 5, 180 },
                    { 568, "MacKenzie friend", 5, 180 },
                    { 569, "Representative", 6, 180 },
                    { 570, "Solicitor", 6, 180 },
                    { 571, "Witness", 5, 180 },
                    // observer (181)
                    { 572, "Observer", 5, 181 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("CaseType", "Id", 33);
            
            for (var i = 147; i < 154; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }
            
            for (var i = 178; i < 182; i++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", i);
            }

            for (var i = 542; i < 573; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
