using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class ChangeHearingVenueForeignKeyToHearingVenueId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearing_HearingVenue_HearingVenueName",
                table: "Hearing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HearingVenue",
                table: "HearingVenue");

            migrationBuilder.DropIndex(
                name: "IX_Hearing_HearingVenueName",
                table: "Hearing");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HearingVenue",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "HearingVenueId",
                table: "Hearing",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HearingVenue",
                table: "HearingVenue",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HearingVenue_Name",
                table: "HearingVenue",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Hearing_HearingVenueId",
                table: "Hearing",
                column: "HearingVenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearing_HearingVenue_HearingVenueId",
                table: "Hearing",
                column: "HearingVenueId",
                principalTable: "HearingVenue",
                principalColumn: "Id");
            
            var sql = "UPDATE Hearing SET HearingVenueId = (SELECT Id FROM HearingVenue WHERE Name = HearingVenueName)";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hearing_HearingVenue_HearingVenueId",
                table: "Hearing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HearingVenue",
                table: "HearingVenue");

            migrationBuilder.DropIndex(
                name: "IX_HearingVenue_Name",
                table: "HearingVenue");

            migrationBuilder.DropIndex(
                name: "IX_Hearing_HearingVenueId",
                table: "Hearing");

            migrationBuilder.DropColumn(
                name: "HearingVenueId",
                table: "Hearing");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HearingVenue",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HearingVenue",
                table: "HearingVenue",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Hearing_HearingVenueName",
                table: "Hearing",
                column: "HearingVenueName");

            migrationBuilder.AddForeignKey(
                name: "FK_Hearing_HearingVenue_HearingVenueName",
                table: "Hearing",
                column: "HearingVenueName",
                principalTable: "HearingVenue",
                principalColumn: "Name");
            
            var sql = "UPDATE Hearing SET HearingVenueId = NULL";
            migrationBuilder.Sql(sql);
        }
    }
}
