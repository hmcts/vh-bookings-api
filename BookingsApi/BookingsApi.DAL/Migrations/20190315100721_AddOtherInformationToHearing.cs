using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddOtherInformationToHearing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "HearingRoomName", table: "Hearing", nullable: true);
            migrationBuilder.AddColumn<string>(name: "OtherInformation", table: "Hearing", nullable: true);

            AddJudgeCaseRoles(migrationBuilder);
            AddJudgeHearingRoles(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 13);
            migrationBuilder.DeleteData("CaseRole", "Id", 5);
            
            migrationBuilder.DropColumn(name: "OtherInformation", table: "Hearing");
            migrationBuilder.DropColumn(name: "HearingRoomName", table: "Hearing");
        }

        public void AddJudgeCaseRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {5, "Judge", (int) CaseRoleGroup.Judge, 1},
                });
        }

        public void AddJudgeHearingRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "CaseRoleId", "UserRoleId", },
                values: new object[,]
                {
                    {13, "Judge", 5, 5},
                });
        }
    }
}