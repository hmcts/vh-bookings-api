using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddUpperChamberHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    {1129, "Applicant", 5, 51},
                    {1130, "Appellant", 5, 51},
                    {1131, "Claimant", 5, 51},
                    {1132, "Respondent", 5, 51},
                    {1133, "Acquiring Authority", 5, 51},
                    {1134, "Compensating Authority", 5, 51},
                    {1135, "Objector", 5, 51},
                    {1136, "Operator", 5, 51},
                    {1137, "Site Providers", 5, 51},
                    {1138, "Respondent Authority", 5, 51},
                    {1139, "interpreter", 5, 51}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 1129; hearingRoleId <= 1139; hearingRoleId++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleId);
            }
        }
    }
}
