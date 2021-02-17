using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddGeneralHearingTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 141, "Daily Test", 3 },
                    { 142, "Demo", 3 },
                    { 143, "Familiarisation", 3 },
                    { 144, "One to one", 3 },
                    { 145, "Test", 3 },
                });
            
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {176, "Claimant", (int) CaseRoleGroup.Claimant, 3},
                    {177, "Defendant", (int) CaseRoleGroup.Defendant, 3},
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // applicant (7)
                    { 510, "Barrister", 6, 7 },
                    { 511, "Expert", 5, 7 },
                    { 512, "Intermediary", 5, 7 },
                    { 513, "Litigation friend", 5, 7 },
                    { 514, "MacKenzie friend", 5, 7 },
                    { 515, "Solicitor", 6, 7 },
                    // claimant (176)
                    { 516, "Barrister", 6, 176 },
                    { 517, "Expert", 5, 176 },
                    { 518, "Intermediary", 5, 176 },
                    { 519, "Interpreter", 5, 176 },
                    { 520, "Litigant in person", 5, 176 },
                    { 521, "Litigation friend", 5, 176 },
                    { 522, "MacKenzie friend", 5, 176 },
                    { 523, "Representative", 6, 176 },
                    { 524, "Solicitor", 6, 176 },
                    { 525, "Witness", 5, 176 },
                    // defendant (177)
                    { 526, "Barrister", 6, 177 },
                    { 527, "Expert", 5, 177 },
                    { 528, "Intermediary", 5, 177 },
                    { 529, "Interpreter", 5, 177 },
                    { 530, "Litigant in person", 5, 177 },
                    { 531, "Litigation friend", 5, 177 },
                    { 532, "MacKenzie friend", 5, 177 },
                    { 533, "Representative", 6, 177 },
                    { 534, "Solicitor", 6, 177 },
                    { 535, "Witness", 5, 177 },
                    // respondent (8)
                    { 536, "Barrister", 6, 8 },
                    { 537, "Expert", 5, 8 },
                    { 538, "Intermediary", 5, 8 },
                    { 539, "Litigation friend", 5, 8 },
                    { 540, "MacKenzie friend", 5, 8 },
                    { 541, "Solicitor", 6, 8 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var i = 141; i < 146; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }
            
            migrationBuilder.DeleteData("CaseRole", "Id", 176);
            migrationBuilder.DeleteData("CaseRole", "Id", 177);
            
            for (var i = 510; i < 542; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
