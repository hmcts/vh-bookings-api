using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalReferenceIdToParticipantAndEndpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalReferenceId",
                table: "Participant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeasuresExternalId",
                table: "Participant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalReferenceId",
                table: "Endpoint",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeasuresExternalId",
                table: "Endpoint",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalReferenceId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "MeasuresExternalId",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "ExternalReferenceId",
                table: "Endpoint");

            migrationBuilder.DropColumn(
                name: "MeasuresExternalId",
                table: "Endpoint");
        }
    }
}
