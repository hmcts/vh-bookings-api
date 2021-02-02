using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class RenameJudgeFirstNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Person SET FirstName = 'Birmingham CFJC' where FirstName LIKE 'Birmingham%'");
            migrationBuilder.Sql("UPDATE Person SET FirstName = 'Manchester CFJC' where FirstName LIKE 'Manchester%'");
            migrationBuilder.Sql("UPDATE Person SET FirstName = 'Taylor House' where FirstName LIKE 'Taylorhouse%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Person SET FirstName = 'Birmingham' where FirstName LIKE 'Birmingham CFJC'");
            migrationBuilder.Sql("UPDATE Person SET FirstName = 'Manchester' where FirstName LIKE 'Manchester CFJC'");
            migrationBuilder.Sql("UPDATE Person SET FirstName = 'Taylorhouse' where FirstName LIKE 'Taylor House'");
        }
    }
}
