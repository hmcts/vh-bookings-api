using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class removeDuplicationFromHearingRoleTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
             table: nameof(HearingRole),
             columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
             values: new object[,]
              {
                    { 15, "Applicant LIP", 5, 7 },
                    { 16, "Solicitor", 6, 7 },
                    { 17, "Respondent LIP", 5, 8 },
                    { 18, "Solicitor", 6, 8 },
                    { 19, "Judge", 4, 9 },
               });

        }
    }
}
