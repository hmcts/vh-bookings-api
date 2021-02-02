using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddDefenceAdvocateToEndpoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint");

            migrationBuilder.AlterColumn<Guid>(
                name: "HearingId",
                table: "Endpoint",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "DefenceAdvocateId",
                table: "Endpoint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_DefenceAdvocateId",
                table: "Endpoint",
                column: "DefenceAdvocateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Participant_DefenceAdvocateId",
                table: "Endpoint",
                column: "DefenceAdvocateId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint",
                column: "HearingId",
                principalTable: "Hearing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Participant_DefenceAdvocateId",
                table: "Endpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_DefenceAdvocateId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "DefenceAdvocateId",
                table: "Endpoint");

            migrationBuilder.AlterColumn<Guid>(
                name: "HearingId",
                table: "Endpoint",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_Hearing_HearingId",
                table: "Endpoint",
                column: "HearingId",
                principalTable: "Hearing",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
