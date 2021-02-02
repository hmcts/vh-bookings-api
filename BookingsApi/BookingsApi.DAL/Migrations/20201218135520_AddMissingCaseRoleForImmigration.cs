using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddMissingCaseRoleForImmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 173, "Judge", (int) CaseRoleGroup.Judge, 32 }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Judge
                    { 378, "Judge", 4, 173 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 378);
            migrationBuilder.DeleteData("CaseRole", "Id", 173);
        }
    }
}
