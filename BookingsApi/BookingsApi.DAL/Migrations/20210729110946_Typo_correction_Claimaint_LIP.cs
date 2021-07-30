using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class Typo_correction_Claimaint_LIP : Migration
    {
        protected void UpdateTypoHearingRole(MigrationBuilder builder, string oldValue, string newValue)
        {
            builder.Sql($"UPDATE HearingRole set [Name] = { newValue } where [Name] = { oldValue } ");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string oldValue = "'Claimaint LIP'";
            string newvalue = "'Claimant LIP'";

            UpdateTypoHearingRole(migrationBuilder, oldValue, newvalue);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string oldValue = "'Claimant LIP'";
            string newvalue = "'Claimaint LIP'";

            UpdateTypoHearingRole(migrationBuilder, oldValue, newvalue);
        }
    }
}
