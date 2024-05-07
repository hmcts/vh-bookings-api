using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddedEndpointParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndpointParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndpointParticipant_Endpoint_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoint",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EndpointParticipant_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndpointParticipant_EndpointId",
                table: "EndpointParticipant",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_EndpointParticipant_ParticipantId",
                table: "EndpointParticipant",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndpointLinkedParticipant",
                columns: table => new
                {
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_EndpointLinkedParticipant_Endpoint_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EndpointLinkedParticipant_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
