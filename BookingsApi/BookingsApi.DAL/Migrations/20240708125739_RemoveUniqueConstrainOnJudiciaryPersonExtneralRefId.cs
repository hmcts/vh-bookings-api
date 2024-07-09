using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstrainOnJudiciaryPersonExtneralRefId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalRefId",
                table: "JudiciaryPerson",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExternalRefId",
                table: "JudiciaryPerson",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson",
                column: "ExternalRefId",
                unique: true,
                filter: "[ExternalRefId] IS NOT NULL");
        }
    }
}
