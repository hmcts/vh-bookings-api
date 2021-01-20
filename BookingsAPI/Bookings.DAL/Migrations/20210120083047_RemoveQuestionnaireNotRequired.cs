using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class RemoveQuestionnaireNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionnaireNotRequired",
                table: "Hearing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QuestionnaireNotRequired",
                table: "Hearing",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
