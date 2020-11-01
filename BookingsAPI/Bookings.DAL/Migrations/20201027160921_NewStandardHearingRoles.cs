using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class NewStandardHearingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "Live"},
                values: new object[,]
                {
                    {175, "Litigant in person", 5, 1, true},
                    {176, "Litigant in person", 5, 2, true},
                    {177, "Litigant in person", 5, 3, true},
                    {178, "Litigant in person", 5, 4, true},
                    {179, "Litigant in person", 5, 7, true},
                    {180, "Litigant in person", 5, 8, true},
                    {181, "Litigant in person", 5, 10, true},
                    {182, "Litigant in person", 5, 11, true},
                    {183, "Litigant in person", 5, 13, true},
                    {184, "Litigant in person", 5, 14, true},
                    {185, "Litigant in person", 5, 16, true},
                    {186, "Litigant in person", 5, 17, true},
                    {187, "Litigant in person", 5, 19, true},
                    {188, "Litigant in person", 5, 20, true},
                    {189, "Litigant in person", 5, 22, true},
                    {190, "Litigant in person", 5, 23, true},
                    {191, "Litigant in person", 5, 25, true},
                    {192, "Litigant in person", 5, 26, true},
                    {193, "Litigant in person", 5, 28, true},
                    {194, "Litigant in person", 5, 29, true},
                    {195, "Litigant in person", 5, 31, true},
                    {196, "Litigant in person", 5, 32, true},
                    {197, "Litigant in person", 5, 34, true},
                    {198, "Litigant in person", 5, 35, true},
                    {199, "Litigant in person", 5, 37, true},
                    {200, "Litigant in person", 5, 38, true},
                    {201, "Litigant in person", 5, 40, true},
                    {202, "Litigant in person", 5, 41, true},
                    {203, "Litigant in person", 5, 43, true},
                    {204, "Litigant in person", 5, 44, true},
                    {205, "Litigant in person", 5, 46, true},
                    {206, "Litigant in person", 5, 47, true},
                    {207, "Litigant in person", 5, 81, true},
                    {208, "Litigant in person", 5, 82, true},
                    {209, "Litigant in person", 5, 83, true},
                    {210, "Litigant in person", 5, 84, true}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 175; hearingRoleID <= 210; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
        }
    }
}
