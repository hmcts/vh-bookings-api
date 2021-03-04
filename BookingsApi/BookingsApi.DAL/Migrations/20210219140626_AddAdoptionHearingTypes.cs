using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddAdoptionHearingTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 37, "Adoption" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 173, "Directions", 37 },
                    { 174, "Full", 37 }
                });
            
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 194, "Appellant", (int) CaseRoleGroup.Appellant, 37 },
                    { 195, "Applicant", (int) CaseRoleGroup.Applicant, 37 },
                    { 196, "Respondent", (int) CaseRoleGroup.Respondent, 37 },
                    { 197, "Observer", (int) CaseRoleGroup.Observer, 37 }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId" },
                new object[,]
                {
                    { 666, "Barrister", 6, 194 },
                    { 667, "Expert", 5, 194 },
                    { 668, "Intermediary", 5, 194 },
                    { 669, "Interpreter", 5, 194 },
                    { 670, "Litigant in person", 5, 194 },
                    { 671, "Litigation friend", 5, 194 },
                    { 672, "MacKenzie friend", 5, 194 },
                    { 673, "Representative", 6, 194 },
                    { 674, "Solicitor", 6, 194 },
                    { 675, "Witness", 5, 194 },

                    { 676, "Barrister", 6, 195 },
                    { 677, "Expert", 5, 195 },
                    { 678, "Intermediary", 5, 195 },
                    { 679, "Interpreter", 5, 195 },
                    { 680, "Litigant in person", 5, 195 },
                    { 681, "Litigation friend", 5, 195 },
                    { 682, "MacKenzie friend", 5, 195 },
                    { 683, "Representative", 6, 195 },
                    { 684, "Solicitor", 6, 195 },
                    { 685, "Witness", 5, 195 },

                    { 686, "Barrister", 6, 196 },
                    { 687, "Expert", 5, 196 },
                    { 688, "Intermediary", 5, 196 },
                    { 689, "Interpreter", 5, 196 },
                    { 690, "Litigant in person", 5, 196 },
                    { 691, "Litigation friend", 5, 196 },
                    { 692, "MacKenzie friend", 5, 196 },
                    { 693, "Representative", 6, 196 },
                    { 694, "Solicitor", 6, 196 },
                    { 695, "Witness", 5, 196 },

                    { 696, "Observer", 5, 197 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(CaseType), "Id", 37);

            for (var i = 173; i < 175; i++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", i);
            }

            for (var i = 194; i < 198; i++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", i);
            }

            for (var i = 666; i < 697; i++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", i);
            }
        }
    }
}
