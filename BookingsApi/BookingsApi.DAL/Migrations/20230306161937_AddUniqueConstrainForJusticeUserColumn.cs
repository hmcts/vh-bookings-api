using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddUniqueConstrainForJusticeUserColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JusticeUser",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JusticeUser_Username",
                table: "JusticeUser",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JusticeUser_Username",
                table: "JusticeUser");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "JusticeUser",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
