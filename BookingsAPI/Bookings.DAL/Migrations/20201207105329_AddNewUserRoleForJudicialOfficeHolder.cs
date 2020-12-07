using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddNewUserRoleForJudicialOfficeHolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO UserRole (Name)
                VALUES ('Judicial Office Holder')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM UserRole WHERE Name = 'Judicial Office Holder'");
        }
    }
}
