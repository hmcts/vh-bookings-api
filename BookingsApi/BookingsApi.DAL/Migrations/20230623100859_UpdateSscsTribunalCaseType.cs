using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateSscsTribunalCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 40,
                column: "ServiceId",
                value: "BBA3");

            migrationBuilder.InsertData(
                table: nameof(Jurisdiction),
                columns: new[] { "Id", "Code", "Name", "IsLive" },
                values: new object[,]
                {
                    { 1, "Social Entitlement Chamber", "Social Entitlement Chamber", true }
                });

            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 40,
                column: "JurisdictionId",
                value: 1);
            
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId", "Code", "WelshName" },
                values: new object[,]
                {
                    { 294, "Substantive", 40, "BBA3-SUB", "Gwrandawiad sylfaenol" },
                    { 295, "Direction Hearings", 40, "BBA3-DIR", "Gwrandawiadau cyfarwyddo" },
                    { 296, "Chambers Outcome", 40, "BBA3-CHA", "Canlyniad y Siambrau" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 40,
                column: "ServiceId",
                value: null);
            
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 40,
                column: "JurisdictionId",
                value: null);
            
            migrationBuilder.DeleteData(
                table: nameof(Jurisdiction),
                keyColumn: "Id",
                keyValue: 40);
            
            for (int hearingTypeId = 294; hearingTypeId <= 296; hearingTypeId++)
            {
                migrationBuilder.DeleteData(
                    table: nameof(HearingType),
                    keyColumn: "Id",
                    keyValue: hearingTypeId);
            }
        }
    }
}
