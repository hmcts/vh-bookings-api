using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddColumnVhoNonAvailabilityTableDeletedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Boolean>(
                name: "Deleted",
                table: "VhoNonAvailability",
                defaultValue: false,
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "VhoNonAvailability");
        }
    }
}
