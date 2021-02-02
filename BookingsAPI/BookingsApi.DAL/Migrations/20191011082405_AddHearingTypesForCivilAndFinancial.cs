using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddHearingTypesForCivilAndFinancial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 13, "First Application", 1 },
                    { 14, "Directions Hearing", 1 },
                    { 15, "Case Management Hearing", 1 },
                    { 16, "Final Hearing", 1 },

                    { 17, "First Application", 2 },
                    { 18, "Directions Hearing", 2 },
                    { 19, "Case Management Hearing", 2 },
                    { 20, "Hearing", 2 },
                    { 21, "Final Hearing", 2 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingType", "Id", 13);
            migrationBuilder.DeleteData("HearingType", "Id", 14);
            migrationBuilder.DeleteData("HearingType", "Id", 15);
            migrationBuilder.DeleteData("HearingType", "Id", 16);
            migrationBuilder.DeleteData("HearingType", "Id", 17);
            migrationBuilder.DeleteData("HearingType", "Id", 18);
            migrationBuilder.DeleteData("HearingType", "Id", 19);
            migrationBuilder.DeleteData("HearingType", "Id", 20);
            migrationBuilder.DeleteData("HearingType", "Id", 21);
        }
    }
}
