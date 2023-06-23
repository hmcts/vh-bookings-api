using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateCivilMoneyClaimsCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceId",
                value: "AAA6");
            
            migrationBuilder.InsertData(
                table: nameof(Jurisdiction),
                columns: new[] { "Id", "Code", "Name", "IsLive" },
                values: new object[,]
                {
                    { 2, "Civil", "Civil", true }
                });
            
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 1,
                column: "JurisdictionId",
                value: 2);
            
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId", "Code" },
                values: new object[,]
                {
                    { 297, "Disposal Hearing", 1, "AAA6-DIS" },
                    { 298, "Trial", 1, "AAA6-TRI" },
                    { 299, "Application Hearings", 1, "AAA6-APP" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceId",
                value: null);
            
            migrationBuilder.UpdateData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 1,
                column: "JurisdictionId",
                value: null);
            
            migrationBuilder.DeleteData(
                table: nameof(Jurisdiction),
                keyColumn: "Id",
                keyValue: 2);
            
            for (int hearingTypeId = 297; hearingTypeId <= 299; hearingTypeId++)
            {
                migrationBuilder.DeleteData(
                    table: nameof(HearingType),
                    keyColumn: "Id",
                    keyValue: hearingTypeId);
            }
        }
    }
}
