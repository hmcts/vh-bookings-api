using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewPropertyCaseTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 9, "Land Registration" },
                    { 10, "Housing Act" },
                    { 11, "Housing & Planning Act" },
                    { 12, "Leasehold Enfranchisement" },
                    { 13, "Leasehold Management" },
                    { 14, "Park Homes" },
                    { 15, "Rents" },
                    { 16, "Right to buy" }
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 26, "Case Management", 9 },
                    { 27, "Substantive hearing", 9 },
                    { 28, "Mediation", 9 },
                    { 29, "Case Management", 10 },
                    { 30, "Substantive hearing", 10 },
                    { 31, "Mediation", 10 },
                    { 32, "Case Management", 11 },
                    { 33, "Substantive hearing", 11 },
                    { 34, "Mediation", 11 },
                    { 35, "Case Management", 12 },
                    { 36, "Substantive hearing", 12 },
                    { 37, "Mediation", 12 },
                    { 38, "Case Management", 13 },
                    { 39, "Substantive hearing", 13 },
                    { 40, "Mediation", 13 },
                    { 41, "Case Management", 14 },
                    { 42, "Substantive hearing", 14 },
                    { 43, "Mediation", 14 },
                    { 44, "Case Management", 15 },
                    { 45, "Substantive hearing", 15 },
                    { 46, "Mediation", 15 },
                    { 47, "Case Management", 16 },
                    { 48, "Substantive hearing", 16 },
                    { 49, "Mediation", 16 },
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 25, "Applicant", (int) CaseRoleGroup.Applicant, 9 },
                    { 26, "Respondent", (int) CaseRoleGroup.Respondent, 9 },
                    { 27, "Judge", (int) CaseRoleGroup.Judge, 9 },

                    { 28, "Applicant", (int) CaseRoleGroup.Applicant, 10 },
                    { 29, "Respondent", (int) CaseRoleGroup.Respondent, 10 },
                    { 30, "Judge", (int) CaseRoleGroup.Judge, 10 },

                    { 31, "Applicant", (int) CaseRoleGroup.Applicant, 11 },
                    { 32, "Respondent", (int) CaseRoleGroup.Respondent, 11 },
                    { 33, "Judge", (int) CaseRoleGroup.Judge, 11 },

                    { 34, "Applicant", (int) CaseRoleGroup.Applicant, 12 },
                    { 35, "Respondent", (int) CaseRoleGroup.Respondent, 12 },
                    { 36, "Judge", (int) CaseRoleGroup.Judge, 12 },

                    { 37, "Applicant", (int) CaseRoleGroup.Applicant, 13 },
                    { 38, "Respondent", (int) CaseRoleGroup.Respondent, 13 },
                    { 39, "Judge", (int) CaseRoleGroup.Judge, 13 },

                    { 40, "Applicant", (int) CaseRoleGroup.Applicant, 14 },
                    { 41, "Respondent", (int) CaseRoleGroup.Respondent, 14 },
                    { 42, "Judge", (int) CaseRoleGroup.Judge, 14 },

                    { 43, "Applicant", (int) CaseRoleGroup.Applicant, 15 },
                    { 44, "Respondent", (int) CaseRoleGroup.Respondent, 15 },
                    { 45, "Judge", (int) CaseRoleGroup.Judge, 15 },

                    { 46, "Applicant", (int) CaseRoleGroup.Applicant, 16 },
                    { 47, "Respondent", (int) CaseRoleGroup.Respondent, 16 },
                    { 48, "Judge", (int) CaseRoleGroup.Judge, 16 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 45, "Applicant LIP", 5, 25 },
                    { 46, "Representative", 6, 25 },
                    { 47, "Respondent LIP", 5, 26 },
                    { 48, "Representative", 6, 26 },
                    { 49, "Judge", 4, 27 },

                    { 50, "Applicant LIP", 5, 28 },
                    { 51, "Representative", 6, 28 },
                    { 52, "Respondent LIP", 5, 29 },
                    { 53, "Representative", 6, 29 },
                    { 54, "Judge", 4, 30 },

                    { 55, "Applicant LIP", 5, 31 },
                    { 56, "Representative", 6, 31 },
                    { 57, "Respondent LIP", 5, 32 },
                    { 58, "Representative", 6, 32 },
                    { 59, "Judge", 4, 33 },

                    { 60, "Applicant LIP", 5, 34 },
                    { 61, "Representative", 6, 34 },
                    { 62, "Respondent LIP", 5, 35 },
                    { 63, "Representative", 6, 35 },
                    { 64, "Judge", 4, 36 },

                    { 65, "Applicant LIP", 5, 37 },
                    { 66, "Representative", 6, 37 },
                    { 67, "Respondent LIP", 5, 38 },
                    { 68, "Representative", 6, 38 },
                    { 69, "Judge", 4, 39 },

                    { 70, "Applicant LIP", 5, 40 },
                    { 71, "Representative", 6, 40 },
                    { 72, "Respondent LIP", 5, 41 },
                    { 73, "Representative", 6, 41 },
                    { 74, "Judge", 4, 42 },

                    { 75, "Applicant LIP", 5, 43 },
                    { 76, "Representative", 6, 43 },
                    { 77, "Respondent LIP", 5, 44 },
                    { 78, "Representative", 6, 44 },
                    { 79, "Judge", 4, 45 },

                    { 80, "Applicant LIP", 5, 46 },
                    { 81, "Representative", 6, 46 },
                    { 82, "Respondent LIP", 5, 47 },
                    { 83, "Representative", 6, 47 },
                    { 84, "Judge", 4, 48 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 45; hearingRoleID <= 84; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }

            for (int CaseRoleRoleID = 25; CaseRoleRoleID <= 48; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }

            for (int HearingTypeId = 26; HearingTypeId <= 49; HearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", HearingTypeId);
            }

            for (int CaseTypeId = 9; CaseTypeId <= 16; CaseTypeId++)
            {
                migrationBuilder.DeleteData("CaseType", "Id", CaseTypeId);
            }
        }
    }
}
