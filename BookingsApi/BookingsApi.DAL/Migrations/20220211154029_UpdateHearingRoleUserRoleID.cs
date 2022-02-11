using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateHearingRoleUserRoleID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(HearingRole),
                keyColumn: "Id",
                keyValue: 857,
                column: "UserRoleId",
                value: UserRoleForHearingRole.UserRoleId["Representative"]);
            migrationBuilder.UpdateData(
                table: nameof(HearingRole),
                keyColumn: "Id",
                keyValue: 858,
                column: "UserRoleId",
                value: UserRoleForHearingRole.UserRoleId["Representative"]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(HearingRole),
                keyColumn: "Id",
                keyValue: 857,
                column: "UserRoleId",
                value: 5);
            migrationBuilder.UpdateData(
                table: nameof(HearingRole),
                keyColumn: "Id",
                keyValue: 858,
                column: "UserRoleId",
                value: 5);
        }
    }
}
