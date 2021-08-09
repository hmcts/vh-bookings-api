using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddUserRoleStaffMember : Migration
    {
        int staffMemberUserId = UserRoleForHearingRole.UserRoleId["Staff Member"];

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(UserRole),
                columns: new[] {"Id", "Name"},
                values: new object[,]
                {
                    {staffMemberUserId, "Staff Member"}
                });
            
            var caseRoleId = 213;
            for (int caseTypeId = 1; caseTypeId <= 38; caseTypeId++)
            {
                migrationBuilder.InsertData(
                    nameof(CaseRole),
                    new[] { "Id", "Name", "Group", "CaseTypeId" },
                    new object[,]
                    {
                        { caseRoleId, "Staff Member", (int) CaseRoleGroup.StaffMember, caseTypeId }
                    });
                caseRoleId++;
            }
            
            var hearingId = 718;
            for (int caseTypeId = 213; caseTypeId <= 250; caseTypeId++)
            {
                migrationBuilder.InsertData(
                    nameof(HearingRole),
                    new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                    new object[,]
                    {
                        { hearingId, "Staff Member", staffMemberUserId, caseTypeId },
                    });
                hearingId++;
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("UserRole", "Id", staffMemberUserId);
            for (int hearingRoleID = 718; hearingRoleID <= 755; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
            for (int CaseRoleRoleID = 213; CaseRoleRoleID <= 250; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }
        }
    }
}
