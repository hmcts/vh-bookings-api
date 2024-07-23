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
            
            migrationBuilder.AddColumn<string>(
                name: "DeletedOn",
                table: "JudiciaryPersonsStaging",
                type: "nvarchar(max)",
                nullable: true);

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
            
            migrationBuilder.AddColumn<string>(
                name: "DeletedOn",
                table: "JudiciaryPerson",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "JudiciaryPersonsStaging");
            
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "JudiciaryPersonsStaging");

            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_Deleted",
                table: "JudiciaryPerson");
            
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "JudiciaryPerson");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "JudiciaryPerson");
        }
    }
}
