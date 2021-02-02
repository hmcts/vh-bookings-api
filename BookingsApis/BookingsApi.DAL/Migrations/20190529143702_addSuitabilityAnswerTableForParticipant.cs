using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class addSuitabilityAnswerTableForParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuitabilityAnswer_Person_PersonId",
                table: "SuitabilityAnswer");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "SuitabilityAnswer",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_SuitabilityAnswer_PersonId",
                table: "SuitabilityAnswer",
                newName: "IX_SuitabilityAnswer_ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuitabilityAnswer_Participant_ParticipantId",
                table: "SuitabilityAnswer",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuitabilityAnswer_Participant_ParticipantId",
                table: "SuitabilityAnswer");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "SuitabilityAnswer",
                newName: "PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_SuitabilityAnswer_ParticipantId",
                table: "SuitabilityAnswer",
                newName: "IX_SuitabilityAnswer_PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuitabilityAnswer_Person_PersonId",
                table: "SuitabilityAnswer",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
