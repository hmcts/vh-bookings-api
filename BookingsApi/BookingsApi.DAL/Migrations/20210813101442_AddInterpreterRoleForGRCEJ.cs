using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddInterpreterRoleForGRCEJ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId"},
                values: new object[,]
                {
                    {756, "Interpreter", UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 203},
                    {757, "Interpreter", UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 204},
                    {758, "Interpreter", UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 208},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 756; hearingRoleId <= 758; hearingRoleId++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleId);
            }
        }
    }
}
