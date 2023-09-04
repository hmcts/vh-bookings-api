using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class MakeParticipantCaseRoleIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_CaseRole_CaseRoleId",
                table: "Participant");

            migrationBuilder.AlterColumn<int>(
                name: "CaseRoleId",
                table: "Participant",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_CaseRole_CaseRoleId",
                table: "Participant",
                column: "CaseRoleId",
                principalTable: "CaseRole",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_CaseRole_CaseRoleId",
                table: "Participant");

            migrationBuilder.AlterColumn<int>(
                name: "CaseRoleId",
                table: "Participant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_CaseRole_CaseRoleId",
                table: "Participant",
                column: "CaseRoleId",
                principalTable: "CaseRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
