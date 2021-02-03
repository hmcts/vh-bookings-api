using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddHearingTypeForTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              nameof(HearingType),
              new[] { "Id", "Name", "CaseTypeId" },
              new object[,]
              {
                    { 50, "Schedule 36 Application", 5 },
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingType", "Id", 50);

        }
    }
}
