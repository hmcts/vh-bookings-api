using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddJobNameAndIsSuccessfulToJobHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuccessful",
                table: "JobHistory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobName",
                table: "JobHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccessful",
                table: "JobHistory");

            migrationBuilder.DropColumn(
                name: "JobName",
                table: "JobHistory");
        }
    }
}
