using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class ChangeLinkedParticipantFkToCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_LinkedParticipant_Participant_LinkedId", "LinkedParticipant");
            migrationBuilder.DropForeignKey("FK_LinkedParticipant_Participant_ParticipantId", "LinkedParticipant");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedParticipant_Participant_LinkedId",
                table: "LinkedParticipant",
                column: "LinkedId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
            );
            
            migrationBuilder.AddForeignKey(
                name: "FK_LinkedParticipant_Participant_ParticipantId",
                table: "LinkedParticipant",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_LinkedParticipant_Participant_LinkedId", "LinkedParticipant");
            migrationBuilder.DropForeignKey("FK_LinkedParticipant_Participant_ParticipantId", "LinkedParticipant");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkedParticipant_Participant_LinkedId",
                table: "LinkedParticipant",
                column: "LinkedId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
            
            migrationBuilder.AddForeignKey(
                name: "FK_LinkedParticipant_Participant_ParticipantId",
                table: "LinkedParticipant",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }
    }
}
