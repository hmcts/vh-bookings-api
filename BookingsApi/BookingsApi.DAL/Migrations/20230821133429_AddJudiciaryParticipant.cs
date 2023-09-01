using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddJudiciaryParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JudiciaryParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JudiciaryPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HearingRoleCode = table.Column<int>(type: "int", nullable: false),
                    HearingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudiciaryParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JudiciaryParticipant_Hearing_HearingId",
                        column: x => x.HearingId,
                        principalTable: "Hearing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JudiciaryParticipant_JudiciaryPerson_JudiciaryPersonId",
                        column: x => x.JudiciaryPersonId,
                        principalTable: "JudiciaryPerson",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryParticipant_HearingId",
                table: "JudiciaryParticipant",
                column: "HearingId");

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryParticipant_JudiciaryPersonId",
                table: "JudiciaryParticipant",
                column: "JudiciaryPersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JudiciaryParticipant");
        }
    }
}
