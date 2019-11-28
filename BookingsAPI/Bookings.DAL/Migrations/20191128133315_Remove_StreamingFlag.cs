using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class Remove_StreamingFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamingFlag",
                table: "Hearing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StreamingFlag",
                table: "Hearing",
                nullable: false,
                defaultValue: false);
        }
    }
}
