using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class UpdateSeedData_JudgeCaseRoleHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId", },
                values: new object[,]
                {
                    {6, "Judge", 0, 2},
                });

            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    {14, "Judge", 4, 6},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 14);
            migrationBuilder.DeleteData("CaseRole", "Id", 6);
        }
    }
}
