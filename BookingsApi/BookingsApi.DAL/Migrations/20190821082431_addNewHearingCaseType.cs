using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class addNewHearingCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseType),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Hearing" },
                });

            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 3, "Hearing", 3 },
                });

            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                     { 7, "Applicant", (int) CaseRoleGroup.Applicant, 3 },
                     { 8, "Respondent", (int) CaseRoleGroup.Respondent, 3 },
                     { 9, "Judge", (int) CaseRoleGroup.Respondent, 3 },
                });

            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    { 15, "Claimant LIP", 5, 1 },
                    { 16, "Solicitor", 6, 1 },
                    { 17, "Defendant LIP", 5, 2 },
                    { 18, "Solicitor", 6, 2 },
                    { 19, "Applicant LIP", 5, 3 },
                    { 20, "Solicitor", 6, 3 },
                    { 21, "Respondent LIP", 5, 4 },
                    { 22, "Solicitor", 6, 4 },
                    { 23, "Judge", 4, 5 },
                    { 24, "Judge", 4, 6 },
                    { 25, "Applicant LIP", 5, 7 },
                    { 26, "Solicitor", 6, 7 },
                    { 27, "Respondent LIP", 5, 8 },
                    { 28, "Solicitor", 6, 8 },
                    { 29, "Judge", 4, 9 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 15);
            migrationBuilder.DeleteData("HearingRole", "Id", 16);
            migrationBuilder.DeleteData("HearingRole", "Id", 17);
            migrationBuilder.DeleteData("HearingRole", "Id", 18);
            migrationBuilder.DeleteData("HearingRole", "Id", 19);
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

            migrationBuilder.DeleteData("CaseRole", "Id", 7);
            migrationBuilder.DeleteData("CaseRole", "Id", 8);
            migrationBuilder.DeleteData("CaseRole", "Id", 9);

            migrationBuilder.DeleteData("HearingType", "Id", 3);

            migrationBuilder.DeleteData("CaseType", "Id", 3);
        }
    }
}
