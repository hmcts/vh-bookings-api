using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveObsoleteV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearing_HearingType_HearingTypeId",
                table: "Hearing");

            migrationBuilder.DropForeignKey(
                name: "FK_HearingRole_CaseRole_CaseRoleId",
                table: "HearingRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_CaseRole_CaseRoleId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_CaseRoleId",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_HearingRole_CaseRoleId",
                table: "HearingRole");

            migrationBuilder.DropIndex(
                name: "IX_Hearing_HearingTypeId",
                table: "Hearing");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Participant_CaseRoleId",
                table: "Participant",
                column: "CaseRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_HearingRole_CaseRoleId",
                table: "HearingRole",
                column: "CaseRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Hearing_HearingTypeId",
                table: "Hearing",
                column: "HearingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseRole_CaseTypeId",
                table: "CaseRole",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HearingType_CaseTypeId",
                table: "HearingType",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HearingType_Code",
                table: "HearingType",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaire_ParticipantId",
                table: "Questionnaire",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_SuitabilityAnswer_QuestionnaireId",
                table: "SuitabilityAnswer",
                column: "QuestionnaireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearing_HearingType_HearingTypeId",
                table: "Hearing",
                column: "HearingTypeId",
                principalTable: "HearingType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HearingRole_CaseRole_CaseRoleId",
                table: "HearingRole",
                column: "CaseRoleId",
                principalTable: "CaseRole",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_CaseRole_CaseRoleId",
                table: "Participant",
                column: "CaseRoleId",
                principalTable: "CaseRole",
                principalColumn: "Id");
        }
    }
}
