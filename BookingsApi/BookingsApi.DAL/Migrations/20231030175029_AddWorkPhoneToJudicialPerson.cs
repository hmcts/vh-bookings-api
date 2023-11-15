using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddWorkPhoneToJudicialPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PersonalCode",
                table: "JudiciaryPerson",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkPhone",
                table: "JudiciaryPerson",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryPerson_PersonalCode",
                table: "JudiciaryPerson",
                column: "PersonalCode",
                unique: true,
                filter: "[PersonalCode] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_PersonalCode",
                table: "JudiciaryPerson");

            migrationBuilder.DropColumn(
                name: "WorkPhone",
                table: "JudiciaryPerson");

            migrationBuilder.AlterColumn<string>(
                name: "PersonalCode",
                table: "JudiciaryPerson",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
