using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class SetWitnessHearingRolesToFalse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingRole SET Live = 0 where Name = 'Witness'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE HearingRole SET Live = 1 where Name = 'Witness'");
        }
    }
}
