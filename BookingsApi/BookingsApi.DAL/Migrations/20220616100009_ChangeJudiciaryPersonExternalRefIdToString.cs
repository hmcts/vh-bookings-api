using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class ChangeJudiciaryPersonExternalRefIdToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalRefId",
                table: "JudiciaryPerson",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson",
                column: "ExternalRefId",
                unique: true,
                filter: "[ExternalRefId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalRefId",
                table: "JudiciaryPerson",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson",
                column: "ExternalRefId",
                unique: true);
        }
    }
}
