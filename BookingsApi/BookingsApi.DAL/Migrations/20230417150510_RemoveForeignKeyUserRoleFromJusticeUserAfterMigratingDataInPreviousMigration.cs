using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class RemoveForeignKeyUserRoleFromJusticeUserAfterMigratingDataInPreviousMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "JusticeUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "JusticeUser",
                nullable: false,
                defaultValue: 0);
        }
    }
}
