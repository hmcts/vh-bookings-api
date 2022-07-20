using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class updateJurisdictionWithFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Jurisdiction",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Jurisdiction",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "JurisdictionId",
                table: "CaseType",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseType_JurisdictionId",
                table: "CaseType",
                column: "JurisdictionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseType_Jurisdiction_JurisdictionId",
                table: "CaseType",
                column: "JurisdictionId",
                principalTable: "Jurisdiction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseType_Jurisdiction_JurisdictionId",
                table: "CaseType");

            migrationBuilder.DropIndex(
                name: "IX_CaseType_JurisdictionId",
                table: "CaseType");

            migrationBuilder.DropColumn(
                name: "JurisdictionId",
                table: "CaseType");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Jurisdiction",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Jurisdiction",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
