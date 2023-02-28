using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddDeletedColumnsForJusticeUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "VhoWorkHours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_VhoWorkHours_Deleted",
                table: "VhoWorkHours",
                column: "Deleted");
            
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "JusticeUser",
                type: "bit",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.CreateIndex(
                name: "IX_JusticeUser_Deleted",
                table: "JusticeUser",
                column: "Deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "VhoWorkHours");
            
            migrationBuilder.DropIndex(
                name: "IX_VhoWorkHours_Deleted",
                table: "VhoWorkHours");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "JusticeUser");
            
            migrationBuilder.DropIndex(
                name: "IX_JusticeUser_Deleted",
                table: "JusticeUser");
        }
    }
}
