using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewHearingRoleAppellantForSSCS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId" },
                new object[,]
                {
                    // Appellant(257)
                    {792, HearingRoles.Appellant, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 257}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 792);
        }
    }
}
