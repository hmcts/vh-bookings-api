using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddDaysOfTheWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "SET IDENTITY_INSERT DayOfWeek ON " +
                "INSERT DayOfWeek (Id, Day) VALUES(1, 'Monday'); " +
                "INSERT DayOfWeek (Id, Day) VALUES(2, 'Tueday'); " +
                "INSERT DayOfWeek (Id, Day) VALUES(3, 'Wednesday'); " +
                "INSERT DayOfWeek (Id, Day) VALUES(4, 'Thursday'); " +
                "INSERT DayOfWeek (Id, Day) VALUES(5, 'Friday'); " +
                "INSERT DayOfWeek (Id, Day) VALUES(6, 'Saturday'); " +
                "INSERT DayOfWeek (Id, Day) VALUES(7, 'Sunday'); " +
                "SET IDENTITY_INSERT DayOfWeek OFF"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE DayOfWeek WHERE Id IN (1, 2, 3, 4, 5, 6, 7)");
        }
    }
}
