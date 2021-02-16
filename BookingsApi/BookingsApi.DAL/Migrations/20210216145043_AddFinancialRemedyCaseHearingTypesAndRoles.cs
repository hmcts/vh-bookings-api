using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddFinancialRemedyCaseHearingTypesAndRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 134, "Financial Dispute Resolution", 2 },
                    { 135, "Interim hearing", 2 },
                });
            
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {174, "Appellant", (int) CaseRoleGroup.Appellant, 2},
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // appellant (174)
                    { 466, "Barrister", 6, 174 },
                    { 467, "Expert", 5, 174 },
                    { 468, "Intermediary", 5, 174 },
                    { 469, "Interpreter", 5, 174 },
                    { 470, "Litigant in person", 5, 174 },
                    { 471, "Litigation friend", 5, 174 },
                    { 472, "MacKenzie friend", 5, 174 },
                    { 473, "Representative", 6, 174 },
                    { 474, "Solicitor", 6, 174 },
                    { 475, "Witness", 5, 174 },
                    // applicant (3)
                    { 476, "Barrister", 6, 3 },
                    { 477, "Expert", 5, 3 },
                    { 478, "Intermediary", 5, 3 },
                    { 479, "Litigation friend", 5, 3 },
                    { 480, "MacKenzie friend", 5, 3 },
                    { 481, "Solicitor", 6, 3 },
                    // respondent (4)
                    { 482, "Barrister", 6, 4 },
                    { 483, "Expert", 5, 4 },
                    { 484, "Intermediary", 5, 4 },
                    { 485, "Litigation friend", 5, 4 },
                    { 486, "MacKenzie friend", 5, 4 },
                    { 487, "Solicitor", 6, 4 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var i = 134; i < 136; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }
            
            migrationBuilder.DeleteData("CaseRole", "Id", 174);
            
            for (var i = 454; i < 476; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
