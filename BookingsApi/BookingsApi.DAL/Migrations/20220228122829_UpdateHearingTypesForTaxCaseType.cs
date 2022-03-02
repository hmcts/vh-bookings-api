using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateHearingTypesForTaxCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingType SET Live = 0 WHERE Name IN ('First Hearing','Final Hearing','Hearing','Directions Hearing','Basic','Standard','Complex','Schedule 36 Application') AND CaseTypeId = 5");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingType SET Live = 1 WHERE Name IN ('First Hearing','Final Hearing','Hearing','Directions Hearing','Basic','Standard','Complex','Schedule 36 Application') AND CaseTypeId = 5");
        }
    }
}
