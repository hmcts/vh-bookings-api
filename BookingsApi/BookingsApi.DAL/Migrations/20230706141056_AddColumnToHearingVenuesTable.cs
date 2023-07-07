using System;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddColumnToHearingVenuesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsScottish",
                table: nameof(HearingVenue),
                type: "bit",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.AddColumn<bool>(
                name: "IsWorkAllocationEnabled",
                table: nameof(HearingVenue),
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScottish",
                table: nameof(HearingVenue));
            
            migrationBuilder.DropColumn(
                name: "IsWorkAllocationEnabled",
                table: nameof(HearingVenue));
        }
    }
}
