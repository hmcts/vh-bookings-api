using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddedUserRoleFkToJusticeUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "JusticeUser");

            migrationBuilder.AddColumn<int>(
                name: "UserRoleId",
                table: "JusticeUser",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JusticeUser_UserRoleId",
                table: "JusticeUser",
                column: "UserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_JusticeUser_UserRole_UserRoleId",
                table: "JusticeUser",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JusticeUser_UserRole_UserRoleId",
                table: "JusticeUser");

            migrationBuilder.DropIndex(
                name: "IX_JusticeUser_UserRoleId",
                table: "JusticeUser");

            migrationBuilder.DropColumn(
                name: "UserRoleId",
                table: "JusticeUser");

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "JusticeUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
