using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedOneToManyRelationshipToEndpointParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_Participant_DefenceAdvocateId",
                table: "Endpoint");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_DefenceAdvocateId",
                table: "Endpoint");
                        
            /* Seperate migration for this
             migrationBuilder.DropColumn(
                name: "DefenceAdvocateId",
                table: "Endpoint");*/

            migrationBuilder.AddColumn<Guid>(
                name: "EndpointId",
                table: "Participant",
                type: "uniqueidentifier",
                nullable: true);

            //Migrate existing data 
            migrationBuilder.Sql(
                @"UPDATE p
            SET p.EndpointId = e.Id
            FROM Participant p
            INNER JOIN Endpoint e ON e.DefenceAdvocateId = p.Id
            WHERE e.DefenceAdvocateId IS NOT NULL;");
            
            migrationBuilder.CreateIndex(
                name: "IX_Participant_EndpointId",
                table: "Participant",
                column: "EndpointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Endpoint_EndpointId",
                table: "Participant",
                column: "EndpointId",
                principalTable: "Endpoint",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Endpoint_EndpointId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_EndpointId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "EndpointId",
                table: "Participant");

            migrationBuilder.AddColumn<Guid>(
                name: "DefenceAdvocateId",
                table: "Endpoint",
                type: "uniqueidentifier",
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
                principalColumn: "Id");
        }
    }
}
