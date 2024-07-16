using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedToJudiciaryPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "JudiciaryPersonsStaging",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "JudiciaryPerson",
                type: "bit",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryPerson_Deleted",
                table: "JudiciaryPerson",
                column: "Deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "JudiciaryPersonsStaging");

            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_Deleted",
                table: "JudiciaryPerson");
            
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "JudiciaryPerson");
        }
    }
}
