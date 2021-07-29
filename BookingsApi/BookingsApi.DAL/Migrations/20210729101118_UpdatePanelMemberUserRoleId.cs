using System.Linq;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdatePanelMemberUserRoleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var newUserRoleId = UserRoleForHearingRole.UserRoleId.FirstOrDefault(k => k.Key == "Panel Member");
            migrationBuilder.UpdateData(
                table: nameof(HearingRole),
                keyColumn: "Id",
                keyValue: 709,
                column: "UserRoleId",
                value: newUserRoleId);
            
            for (int hearingRoleID = 714; hearingRoleID <= 717; hearingRoleID++)
            {
                migrationBuilder.UpdateData(
                    table: nameof(HearingRole),
                    keyColumn: "Id",
                    keyValue: hearingRoleID,
                    column: "UserRoleId",
                    value: newUserRoleId);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: nameof(HearingRole),
                keyColumn: "Id",
                keyValue: 709,
                column: "UserRoleId",
                value: 5);
            
            for (int hearingRoleID = 714; hearingRoleID <= 717; hearingRoleID++)
            {
                migrationBuilder.UpdateData(
                    table: nameof(HearingRole),
                    keyColumn: "Id",
                    keyValue: hearingRoleID,
                    column: "UserRoleId",
                    value: 5);
            }
        }
    }
}
