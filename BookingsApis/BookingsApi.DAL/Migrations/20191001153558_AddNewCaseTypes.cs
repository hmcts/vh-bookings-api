using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewCaseTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE CaseType SET Name = 'Generic' WHERE Name = 'Hearing'");

            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    { 4, "Children Act" },
                    { 5, "Tax" },
                    { 6, "Family Law Act" },
                    { 7, "Tribunal" }
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 4, "Hearing", 4 },
                    { 5, "First Hearing", 5 },
                    { 6, "Substantive Hearing", 5 },
                    { 7, "Case Management", 5 },
                    { 8, "Directions Hearing", 5 },
                    { 9, "Hearing", 5 },
                    { 10, "Final Hearing", 5 },
                    { 11, "Hearing", 6 },
                    { 12, "Hearing", 7 }
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 10, "Applicant", (int) CaseRoleGroup.Applicant, 4 },
                    { 11, "Respondent", (int) CaseRoleGroup.Respondent, 4 },
                    { 12, "Judge", (int) CaseRoleGroup.Respondent, 4 },

                    { 13, "Applicant", (int) CaseRoleGroup.Applicant, 5 },
                    { 14, "Respondent", (int) CaseRoleGroup.Respondent, 5 },
                    { 15, "Judge", (int) CaseRoleGroup.Respondent, 5 },

                    { 16, "Applicant", (int) CaseRoleGroup.Applicant, 6 },
                    { 17, "Respondent", (int) CaseRoleGroup.Respondent, 6 },
                    { 18, "Judge", (int) CaseRoleGroup.Respondent, 6 },

                    { 19, "Applicant", (int) CaseRoleGroup.Applicant, 7 },
                    { 20, "Respondent", (int) CaseRoleGroup.Respondent, 7 },
                    { 21, "Judge", (int) CaseRoleGroup.Respondent, 7 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 20, "Applicant LIP", 5, 10 },
                    { 21, "Solicitor", 6, 10 },
                    { 22, "Respondent LIP", 5, 11 },
                    { 23, "Solicitor", 6, 11 },
                    { 24, "Judge", 4, 12 },

                    { 25, "Applicant LIP", 5, 13 },
                    { 26, "Solicitor", 6, 13 },
                    { 27, "Respondent LIP", 5, 14 },
                    { 28, "Solicitor", 6, 14 },
                    { 29, "Judge", 4, 15 },

                    { 30, "Applicant LIP", 5, 16 },
                    { 31, "Solicitor", 6, 16 },
                    { 32, "Respondent LIP", 5, 17 },
                    { 33, "Solicitor", 6, 17 },
                    { 34, "Judge", 4, 18 },

                    { 35, "Applicant LIP", 5, 19 },
                    { 36, "Solicitor", 6, 19 },
                    { 37, "Respondent LIP", 5, 20 },
                    { 38, "Solicitor", 6, 20 },
                    { 39, "Judge", 4, 21 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 20);
            migrationBuilder.DeleteData("HearingRole", "Id", 21);
            migrationBuilder.DeleteData("HearingRole", "Id", 22);
            migrationBuilder.DeleteData("HearingRole", "Id", 23);
            migrationBuilder.DeleteData("HearingRole", "Id", 24);
            migrationBuilder.DeleteData("HearingRole", "Id", 25);
            migrationBuilder.DeleteData("HearingRole", "Id", 26);
            migrationBuilder.DeleteData("HearingRole", "Id", 27);
            migrationBuilder.DeleteData("HearingRole", "Id", 28);
            migrationBuilder.DeleteData("HearingRole", "Id", 29);
            migrationBuilder.DeleteData("HearingRole", "Id", 30);
            migrationBuilder.DeleteData("HearingRole", "Id", 31);
            migrationBuilder.DeleteData("HearingRole", "Id", 32);
            migrationBuilder.DeleteData("HearingRole", "Id", 33);
            migrationBuilder.DeleteData("HearingRole", "Id", 34);
            migrationBuilder.DeleteData("HearingRole", "Id", 35);
            migrationBuilder.DeleteData("HearingRole", "Id", 36);
            migrationBuilder.DeleteData("HearingRole", "Id", 37);
            migrationBuilder.DeleteData("HearingRole", "Id", 38);
            migrationBuilder.DeleteData("HearingRole", "Id", 39);

            migrationBuilder.DeleteData("CaseRole", "Id", 10);
            migrationBuilder.DeleteData("CaseRole", "Id", 11);
            migrationBuilder.DeleteData("CaseRole", "Id", 12);
            migrationBuilder.DeleteData("CaseRole", "Id", 13);
            migrationBuilder.DeleteData("CaseRole", "Id", 14);
            migrationBuilder.DeleteData("CaseRole", "Id", 15);
            migrationBuilder.DeleteData("CaseRole", "Id", 16);
            migrationBuilder.DeleteData("CaseRole", "Id", 17);
            migrationBuilder.DeleteData("CaseRole", "Id", 18);
            migrationBuilder.DeleteData("CaseRole", "Id", 19);
            migrationBuilder.DeleteData("CaseRole", "Id", 20);
            migrationBuilder.DeleteData("CaseRole", "Id", 21);

            migrationBuilder.DeleteData("HearingType", "Id", 4);
            migrationBuilder.DeleteData("HearingType", "Id", 5);
            migrationBuilder.DeleteData("HearingType", "Id", 6);
            migrationBuilder.DeleteData("HearingType", "Id", 7);
            migrationBuilder.DeleteData("HearingType", "Id", 8);
            migrationBuilder.DeleteData("HearingType", "Id", 9);
            migrationBuilder.DeleteData("HearingType", "Id", 10);
            migrationBuilder.DeleteData("HearingType", "Id", 11);
            migrationBuilder.DeleteData("HearingType", "Id", 12);

            migrationBuilder.Sql("UPDATE CaseType SET Name = 'Hearing' WHERE Name = 'Generic'");

            migrationBuilder.DeleteData("CaseType", "Id", 4);
            migrationBuilder.DeleteData("CaseType", "Id", 5);
            migrationBuilder.DeleteData("CaseType", "Id", 6);
            migrationBuilder.DeleteData("CaseType", "Id", 7);
        }
    }
}
