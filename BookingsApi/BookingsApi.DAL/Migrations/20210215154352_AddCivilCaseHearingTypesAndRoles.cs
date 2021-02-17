using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCivilCaseHearingTypesAndRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 121, "Case Management Hearing", 8 },
                    { 122, "Costs", 8 },
                    { 123, "Enforcement Hearing", 8 },
                    { 124, "General Application", 8 },
                    { 125, "Infant Settlement", 8 },
                    { 126, "Injunction", 8 },
                    { 127, "Insolvency", 8 },
                    { 128, "Multi Track Trial", 8 },
                    { 129, "Part 8 (General)", 8 },
                    { 130, "Possession Hearing", 8 },
                    { 131, "Return of Goods", 8 },
                    { 132, "Small Claim Trial", 8 },
                    { 133, "Stage 3 Part 8 Hearing", 8 },
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // claimant (22)
                    { 454, "Barrister", 6, 22 },
                    { 455, "Expert", 5, 22 },
                    { 456, "Intermediary", 5, 22 },
                    { 457, "Litigation friend", 5, 22 },
                    { 458, "MacKenzie friend", 5, 22 },
                    { 459, "Solicitor", 6, 22 },
                    // defendant (23)
                    { 460, "Barrister", 6, 23 },
                    { 461, "Expert", 5, 23 },
                    { 462, "Intermediary", 5, 23 },
                    { 463, "Litigation friend", 5, 23 },
                    { 464, "MacKenzie friend", 5, 23 },
                    { 465, "Solicitor", 6, 23 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var i = 121; i < 134; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }
            
            for (var i = 379; i < 389; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
