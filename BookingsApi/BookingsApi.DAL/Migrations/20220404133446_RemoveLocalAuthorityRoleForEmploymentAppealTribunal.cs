using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class RemoveLocalAuthorityRoleForEmploymentAppealTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 922);
            migrationBuilder.DeleteData("HearingRole", "Id", 936);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "HearingRole",
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "Live" },
                new object[,]
                {
                    {936, "Local Authority", 5, 295, true},
                    {922, "Local Authority", 5, 294, true}
                });
        }
    }
}
