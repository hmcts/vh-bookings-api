using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddConferenceSupplierColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConferenceSupplier",
                table: "Hearing",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConferenceSupplier",
                table: "Hearing");
        }
    }
}
