using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class add_case_type_court_of_appeal_criminal_division : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 18, "Court of Appeal Criminal Division" }
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 58, "FC Appeal", 18 },
                    { 59, "FC Application", 18 },
                    { 60, "Directions", 18 },
                    { 61, "Appn to treat abandonment as a nullity", 18 },
                    { 62, "Section 13", 18 },
                    { 63, "PII on notice", 18 },
                    { 64, "PII not on notice", 18 },
                    { 65, "Costs application", 18 },
                    { 66, "SC pronouncement", 18 },
                    { 67, "Reserved Judgment", 18 },
                    { 68, "Reasons for Judgment", 18 },
                    { 69, "Hand down Judgment", 18 },
                    { 70, "Appn to reopen case", 18 },
                    { 71, "Appn for leave to appeal to SC", 18 },
                    { 72, "Order of SC to be made order of CACD", 18 },
                    { 73, "Ref for summary dismissal under S20", 18 },
                    { 74, "For mention", 18 },
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 88, "None", (int) CaseRoleGroup.None, 18 },
                    { 89, "Judge", (int) CaseRoleGroup.Judge, 18 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 128, "Appellant", 5, 88 },
                    { 129, "Prosecution", 6, 88 },
                    { 130, "Defence Advocate", 6, 88 },
                    { 131, "Prosecution Advocate", 6, 88 },
                    { 132, "Winger", 5, 88 },
                    { 133, "Witness", 5, 88 },
                    { 134, "Interpreter", 5, 88 },
                    { 135, "MacKenzie friend", 5, 88 },
                    { 136, "Expert", 5, 88 },
                    { 137, "Observer", 5, 88 },
                    { 138, "Judge", 4, 89 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 128; hearingRoleID <= 138; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }

            for (int CaseRoleRoleID = 88; CaseRoleRoleID <= 89; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }

            for (int HearingTypeId = 58; HearingTypeId <= 74; HearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", HearingTypeId);
            }

            migrationBuilder.DeleteData("CaseType", "Id", 18);
        }
    }
}


