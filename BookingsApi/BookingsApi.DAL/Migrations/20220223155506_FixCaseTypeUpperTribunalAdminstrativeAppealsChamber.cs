using BookingsApi.Contract.V1.Enums;
using BookingsApi.DAL.Helper;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class FixCaseTypeUpperTribunalAdminstrativeAppealsChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               nameof(CaseRole),
               new[] { "Id", "Name", "Group", "CaseTypeId" },
               new object[,]
               {
                    { 299, "Judge", (int) CaseRoleGroup.Judge, 46 },
                  
               });

            migrationBuilder.InsertData(
               nameof(HearingRole),
               new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
               new object[,]
               {
                    // Judge(299)
                    {944, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 299},
           });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 944);
            migrationBuilder.DeleteData(nameof(CaseRole), "Id", 299);
        }
    }
}
