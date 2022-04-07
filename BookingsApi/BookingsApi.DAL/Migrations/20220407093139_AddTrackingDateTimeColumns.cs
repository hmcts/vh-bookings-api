using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddTrackingDateTimeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "UserRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Organisation",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Organisation",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LinkedParticipant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "LinkedParticipant",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "HearingVenue",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "HearingVenue",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "HearingType",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "HearingType",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "HearingRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "HearingRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "HearingCase",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "HearingCase",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Endpoint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Endpoint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CaseType",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CaseType",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CaseRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "CaseRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Case",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Case",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Organisation");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Organisation");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LinkedParticipant");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "LinkedParticipant");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "HearingVenue");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "HearingVenue");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "HearingType");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "HearingType");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "HearingRole");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "HearingRole");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "HearingCase");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "HearingCase");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CaseType");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CaseType");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CaseRole");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "CaseRole");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Case");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Case");
        }
    }
}
