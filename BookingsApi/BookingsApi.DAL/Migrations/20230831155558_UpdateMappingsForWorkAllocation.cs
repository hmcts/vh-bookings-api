using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateMappingsForWorkAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JusticeUserRoles_JusticeUser_JusticeUserId",
                table: "JusticeUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_JusticeUserRoles_UserRole_UserRoleId",
                table: "JusticeUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Allocation_JusticeUserId",
                table: "Allocation");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "JusticeUserRoles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "JusticeUserId",
                table: "JusticeUserRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allocation_JusticeUserId_HearingId",
                table: "Allocation",
                columns: new[] { "JusticeUserId", "HearingId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JusticeUserRoles_JusticeUser_JusticeUserId",
                table: "JusticeUserRoles",
                column: "JusticeUserId",
                principalTable: "JusticeUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JusticeUserRoles_UserRole_UserRoleId",
                table: "JusticeUserRoles",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JusticeUserRoles_JusticeUser_JusticeUserId",
                table: "JusticeUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_JusticeUserRoles_UserRole_UserRoleId",
                table: "JusticeUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Allocation_JusticeUserId_HearingId",
                table: "Allocation");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "JusticeUserRoles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "JusticeUserId",
                table: "JusticeUserRoles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Allocation_JusticeUserId",
                table: "Allocation",
                column: "JusticeUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JusticeUserRoles_JusticeUser_JusticeUserId",
                table: "JusticeUserRoles",
                column: "JusticeUserId",
                principalTable: "JusticeUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JusticeUserRoles_UserRole_UserRoleId",
                table: "JusticeUserRoles",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id");
        }
    }
}
