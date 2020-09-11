using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddFieldDefenceAdvocateToEndpointTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Participant_DefenceAdvocateId",
                table: "Endpoint");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_DefenceAdvocateId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "DefenceAdvocateId",
                table: "Endpoint");
        }
    }
}
