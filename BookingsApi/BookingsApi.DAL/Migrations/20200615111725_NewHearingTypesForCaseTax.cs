using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class NewHearingTypesForCaseTax : Migration
    {
protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 23, "Basic", 5 },
                    { 24, "Standard", 5 },
                    { 25, "Complex", 5 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingType", "Id", 23);
            migrationBuilder.DeleteData("HearingType", "Id", 24);
            migrationBuilder.DeleteData("HearingType", "Id", 25);
        }
    }
}
