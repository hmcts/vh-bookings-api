using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInterpreterLanguagePropertiesToParticipantsAndEndpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterpreterLanguageId",
                table: "Participant",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLanguage",
                table: "Participant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterpreterLanguageId",
                table: "JudiciaryParticipant",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLanguage",
                table: "JudiciaryParticipant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterpreterLanguageId",
                table: "Endpoint",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherLanguage",
                table: "Endpoint",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_InterpreterLanguageId",
                table: "Participant",
                column: "InterpreterLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryParticipant_InterpreterLanguageId",
                table: "JudiciaryParticipant",
                column: "InterpreterLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_InterpreterLanguageId",
                table: "Endpoint",
                column: "InterpreterLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Endpoint_InterpreterLanguage_InterpreterLanguageId",
                table: "Endpoint",
                column: "InterpreterLanguageId",
                principalTable: "InterpreterLanguage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JudiciaryParticipant_InterpreterLanguage_InterpreterLanguageId",
                table: "JudiciaryParticipant",
                column: "InterpreterLanguageId",
                principalTable: "InterpreterLanguage",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_InterpreterLanguage_InterpreterLanguageId",
                table: "Participant",
                column: "InterpreterLanguageId",
                principalTable: "InterpreterLanguage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endpoint_InterpreterLanguage_InterpreterLanguageId",
                table: "Endpoint");

            migrationBuilder.DropForeignKey(
                name: "FK_JudiciaryParticipant_InterpreterLanguage_InterpreterLanguageId",
                table: "JudiciaryParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_InterpreterLanguage_InterpreterLanguageId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_InterpreterLanguageId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_JudiciaryParticipant_InterpreterLanguageId",
                table: "JudiciaryParticipant");

            migrationBuilder.DropIndex(
                name: "IX_Endpoint_InterpreterLanguageId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "InterpreterLanguageId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "OtherLanguage",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "InterpreterLanguageId",
                table: "JudiciaryParticipant");

            migrationBuilder.DropColumn(
                name: "OtherLanguage",
                table: "JudiciaryParticipant");

            migrationBuilder.DropColumn(
                name: "InterpreterLanguageId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "OtherLanguage",
                table: "Endpoint");
        }
    }
}
