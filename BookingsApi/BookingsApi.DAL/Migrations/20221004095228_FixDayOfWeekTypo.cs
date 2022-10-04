using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class FixDayOfWeekTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE DayOfWeek SET Day = 'Tuesday' WHERE Id = 2; "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE DayOfWeek SET Day = 'Tueday' WHERE Id = 2; "
            );
        }
    }
}
