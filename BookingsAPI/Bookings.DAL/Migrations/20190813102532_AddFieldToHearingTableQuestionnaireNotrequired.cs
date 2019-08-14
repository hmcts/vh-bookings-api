using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddFieldToHearingTableQuestionnaireNotrequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QuestionnaireNotRequired",
                table: "Hearing",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionnaireNotRequired",
                table: "Hearing");
        }
    }
}
