using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class RemoveQiestionnaireIdFromParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Questionnaire_QuestionnaireId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire");

            migrationBuilder.DropIndex(
                name: "IX_Participant_QuestionnaireId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "Participant");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire",
                column: "ParticipantId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire");

            migrationBuilder.AddColumn<long>(
                name: "QuestionnaireId",
                table: "Participant",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_QuestionnaireId",
                table: "Participant",
                column: "QuestionnaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Questionnaire_QuestionnaireId",
                table: "Participant",
                column: "QuestionnaireId",
                principalTable: "Questionnaire",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
