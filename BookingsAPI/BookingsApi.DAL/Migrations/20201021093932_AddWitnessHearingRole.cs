using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddWitnessHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    {139, "Witness", 5, 1},
                    {140, "Witness", 5, 2},
                    {141, "Witness", 5, 3},
                    {142, "Witness", 5, 4},
                    {143, "Witness", 5, 7},
                    {144, "Witness", 5, 8},
                    {145, "Witness", 5, 10},
                    {146, "Witness", 5, 11},
                    {147, "Witness", 5, 13},
                    {148, "Witness", 5, 14},
                    {149, "Witness", 5, 16},
                    {150, "Witness", 5, 17},
                    {151, "Witness", 5, 19},
                    {152, "Witness", 5, 20},
                    {153, "Witness", 5, 22},
                    {154, "Witness", 5, 23},
                    {155, "Witness", 5, 25},
                    {156, "Witness", 5, 26},
                    {157, "Witness", 5, 28},
                    {158, "Witness", 5, 29},
                    {159, "Witness", 5, 31},
                    {160, "Witness", 5, 32},
                    {161, "Witness", 5, 34},
                    {162, "Witness", 5, 35},
                    {163, "Witness", 5, 37},
                    {164, "Witness", 5, 38},
                    {165, "Witness", 5, 40},
                    {166, "Witness", 5, 41},
                    {167, "Witness", 5, 43},
                    {168, "Witness", 5, 44},
                    {169, "Witness", 5, 46},
                    {170, "Witness", 5, 47},
                    {171, "Witness", 5, 81},
                    {172, "Witness", 5, 82},
                    {173, "Witness", 5, 83},
                    {174, "Witness", 5, 84}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 139; hearingRoleID <= 174; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
        }
    }
}
