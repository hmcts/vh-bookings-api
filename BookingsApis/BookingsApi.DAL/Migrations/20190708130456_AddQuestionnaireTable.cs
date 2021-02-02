using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddQuestionnaireTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuitabilityAnswer_Participant_ParticipantId",
                table: "SuitabilityAnswer");

            migrationBuilder.DropIndex(
                name: "IX_SuitabilityAnswer_ParticipantId",
                table: "SuitabilityAnswer");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "SuitabilityAnswer");

            migrationBuilder.AddColumn<long>(
                name: "QuestionnaireId",
                table: "SuitabilityAnswer",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "QuestionnaireId",
                table: "Participant",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Questionnaire",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<Guid>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questionnaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questionnaire_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuitabilityAnswer_QuestionnaireId",
                table: "SuitabilityAnswer",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_QuestionnaireId",
                table: "Participant",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Questionnaire_QuestionnaireId",
                table: "Participant",
                column: "QuestionnaireId",
                principalTable: "Questionnaire",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SuitabilityAnswer_Questionnaire_QuestionnaireId",
                table: "SuitabilityAnswer",
                column: "QuestionnaireId",
                principalTable: "Questionnaire",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Questionnaire_QuestionnaireId",
                table: "Participant");

            migrationBuilder.DropForeignKey(
                name: "FK_SuitabilityAnswer_Questionnaire_QuestionnaireId",
                table: "SuitabilityAnswer");

            migrationBuilder.DropTable(
                name: "Questionnaire");

            migrationBuilder.DropIndex(
                name: "IX_SuitabilityAnswer_QuestionnaireId",
                table: "SuitabilityAnswer");

            migrationBuilder.DropIndex(
                name: "IX_Participant_QuestionnaireId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "SuitabilityAnswer");

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "Participant");

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "SuitabilityAnswer",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SuitabilityAnswer_ParticipantId",
                table: "SuitabilityAnswer",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuitabilityAnswer_Participant_ParticipantId",
                table: "SuitabilityAnswer",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
