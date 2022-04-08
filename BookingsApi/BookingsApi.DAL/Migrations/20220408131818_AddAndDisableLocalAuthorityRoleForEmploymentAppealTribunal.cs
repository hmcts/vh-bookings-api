using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddAndDisableLocalAuthorityRoleForEmploymentAppealTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "HearingRole",
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "Live" },
                new object[,]
                {
                    {936, "Local Authority", 5, 295, false},
                    {922, "Local Authority", 5, 294, false}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 922);
            migrationBuilder.DeleteData("HearingRole", "Id", 936);
        }
    }
}
