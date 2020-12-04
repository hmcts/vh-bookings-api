using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddJudicialOfficeHolderHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HearingRole",
                columns: new[] {"Name", "UserRoleId", "CaseRoleId"},
                values: new object[,]
                {
                    {"Winger", 7, 88 },
                    {"Panel Member", 7, 50 },
                    {"Panel Member", 7, 52 },
                    {"Panel Member", 7, 54 },
                    {"Panel Member", 7, 56 },
                    {"Panel Member", 7, 58 },
                    {"Panel Member", 7, 60 },
                    {"Panel Member", 7, 62 },
                    {"Panel Member", 7, 64 },
                    {"Panel Member", 7, 66 },
                    {"Panel Member", 7, 68 },
                    {"Panel Member", 7, 70 },
                    {"Panel Member", 7, 72 },
                    {"Panel Member", 7, 74 },
                    {"Panel Member", 7, 76 },
                    {"Panel Member", 7, 78 },
                    {"Panel Member", 7, 80 },
                    {"Panel Member", 7, 87 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (var hearingRoleId = 214; hearingRoleId < 233; hearingRoleId++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleId);
            }
        }
    }
}
