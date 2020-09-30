using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddHearingIdForeignKeyForEndpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint");

            migrationBuilder.AlterColumn<Guid>(
                name: "HearingId",
                table: "Endpoint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint",
                column: "HearingId",
                principalTable: "Hearing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint");

            migrationBuilder.AlterColumn<Guid>(
                name: "HearingId",
                table: "Endpoint",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint",
                column: "HearingId",
                principalTable: "Hearing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
