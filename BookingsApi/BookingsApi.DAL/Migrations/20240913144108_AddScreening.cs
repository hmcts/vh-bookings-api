using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddScreening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScreeningId",
                table: "Participant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ScreeningId",
                table: "Endpoint",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Screening",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screening", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Screening_Endpoint_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoint",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Screening_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScreeningEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreeningId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreeningEntity_Endpoint_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoint",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScreeningEntity_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScreeningEntity_Screening_ScreeningId",
                        column: x => x.ScreeningId,
                        principalTable: "Screening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Screening_EndpointId",
                table: "Screening",
                column: "EndpointId",
                unique: true,
                filter: "[EndpointId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Screening_ParticipantId",
                table: "Screening",
                column: "ParticipantId",
                unique: true,
                filter: "[ParticipantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningEntity_EndpointId",
                table: "ScreeningEntity",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningEntity_ParticipantId",
                table: "ScreeningEntity",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningEntity_ScreeningId",
                table: "ScreeningEntity",
                column: "ScreeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScreeningEntity");

            migrationBuilder.DropTable(
                name: "Screening");

            migrationBuilder.DropColumn(
                name: "ScreeningId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "ScreeningId",
                table: "Endpoint");
        }
    }
}
