using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class RemoveForeignKeyUserRoleFromJusticeUserAfterMigratingDataInPreviousMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRoleId",
                table: "JusticeUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserRoleId",
                table: "JusticeUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
