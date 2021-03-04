using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddPlacementCaseHearingTypesAndRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 36, "Placement" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 171, "Directions", 36 },
                    { 172, "Full", 36 }
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 190, "Appellant", (int) CaseRoleGroup.Appellant, 36 },
                    { 191, "Applicant", (int) CaseRoleGroup.Applicant, 36 },
                    { 192, "Respondent", (int) CaseRoleGroup.Respondent, 36 },
                    { 193, "Observer", (int) CaseRoleGroup.Observer, 36 }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId" },
                new object[,]
                {
                    { 635, "Barrister", 6, 190 },
                    { 636, "Expert", 5, 190 },
                    { 637, "Intermediary", 5, 190 },
                    { 638, "Interpreter", 5, 190 },
                    { 639, "Litigant in person", 5, 190 },
                    { 640, "Litigation friend", 5, 190 },
                    { 641, "MacKenzie friend", 5, 190 },
                    { 642, "Representative", 6, 190 },
                    { 643, "Solicitor", 6, 190 },
                    { 644, "Witness", 5, 190 },

                    { 645, "Barrister", 6, 191 },
                    { 646, "Expert", 5, 191 },
                    { 647, "Intermediary", 5, 191 },
                    { 648, "Interpreter", 5, 191 },
                    { 649, "Litigant in person", 5, 191 },
                    { 650, "Litigation friend", 5, 191 },
                    { 651, "MacKenzie friend", 5, 191 },
                    { 652, "Representative", 6, 191 },
                    { 653, "Solicitor", 6, 191 },
                    { 654, "Witness", 5, 191 },

                    { 655, "Barrister", 6, 192 },
                    { 656, "Expert", 5, 192 },
                    { 657, "Intermediary", 5, 192 },
                    { 658, "Interpreter", 5, 192 },
                    { 659, "Litigant in person", 5, 192 },
                    { 660, "Litigation friend", 5, 192 },
                    { 661, "MacKenzie friend", 5, 192 },
                    { 662, "Representative", 6, 192 },
                    { 663, "Solicitor", 6, 192 },
                    { 664, "Witness", 5, 192 },

                    { 665, "Observer", 5, 193 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 36);

            for (var i = 171; i < 173; i++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", i);
            }

            for (var i = 190; i < 194; i++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", i);
            }

            for (var i = 635; i < 666; i++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", i);
            }
        }
    }
}
