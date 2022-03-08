using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddHearingTypesForCaseTypeTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              nameof(HearingType),
              new[] { "Id", "Name", "CaseTypeId" },
              new object[,]
              {
                    { 267, "Interim Application", 5 },
                    { 268, "Substantive Application", 5 },
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(HearingType), "Id", 267);
            migrationBuilder.DeleteData(nameof(HearingType), "Id", 268);
        }
    }
}
