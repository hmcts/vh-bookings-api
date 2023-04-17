using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdatedJusticeUser_to_have_multiple_UserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "VhoWorkHours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JusticeUser",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "JusticeUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JusticeUserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JusticeUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserRoleId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JusticeUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JusticeUserRoles_JusticeUser_JusticeUserId",
                        column: x => x.JusticeUserId,
                        principalTable: "JusticeUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JusticeUserRoles_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JusticeUser_Username",
                table: "JusticeUser",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JusticeUserRoles_JusticeUserId",
                table: "JusticeUserRoles",
                column: "JusticeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JusticeUserRoles_UserRoleId",
                table: "JusticeUserRoles",
                column: "UserRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JusticeUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_JusticeUser_Username",
                table: "JusticeUser");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "VhoWorkHours");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "JusticeUser");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JusticeUser",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserRoleId",
                table: "JusticeUser",
                type: "int",
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
    }
}
