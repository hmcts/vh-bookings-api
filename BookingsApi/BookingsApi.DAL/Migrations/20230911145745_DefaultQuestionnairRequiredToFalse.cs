using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class DefaultQuestionnairRequiredToFalse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire");

            migrationBuilder.AlterColumn<bool>(
                name: "QuestionnaireNotRequired",
                table: "Hearing",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire");

            migrationBuilder.AlterColumn<bool>(
                name: "QuestionnaireNotRequired",
                table: "Hearing",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire",
                column: "ParticipantId",
                unique: true);
        }
    }
}
