using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddJudgeRoleForNewHearingTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 198, "Judge", (int) CaseRoleGroup.Judge, 33 },
                    { 199, "Judge", (int) CaseRoleGroup.Judge, 34 },
                    { 200, "Judge", (int) CaseRoleGroup.Judge, 35 },
                    { 201, "Judge", (int) CaseRoleGroup.Judge, 36 },
                    { 202, "Judge", (int) CaseRoleGroup.Judge, 37 },
                });
                
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId" },
                new object[,]
                {
                    { 697, "Judge", 4, 198 },
                    { 698, "Judge", 4, 199 },
                    { 699, "Judge", 4, 200 },
                    { 700, "Judge", 4, 201 },
                    { 701, "Judge", 4, 202 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var i = 198; i < 203; i++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", i);
            }
            
            for (var i = 697; i < 702; i++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", i);
            }
        }
    }
}
