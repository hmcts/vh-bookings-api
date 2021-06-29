using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseTypeForGRCEJ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
             * After serveral discussions we have decided to continue hard coding the ids similar to other migration
             * However if we continue to get tasks to update table data perhaps should look for alternative in the future
             * Possible solutions might be to use, migrationBuilder.Sql(), passing a sql command to query the id of the new group
             * and then using it in the necessary tables. 
             */

            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {38, "GRC - EJ"}
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {175, "Case Management Hearing", 38},
                    {176, "Final Hearing", 38},
                    {177, "Costs Hearing", 38}
                });
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 203, "Applicant", (int) CaseRoleGroup.Applicant, 38 },
                    { 204, "Respondent", (int) CaseRoleGroup.Respondent, 38 },
                    { 205, "Judge", (int) CaseRoleGroup.Judge, 38 },
                    { 206, "Panel Member", (int) CaseRoleGroup.PanelMember, 38 },
                    { 207, "Observer", (int) CaseRoleGroup.Observer, 38 },
                    { 208, "Appellant", (int) CaseRoleGroup.Appellant, 38 },
                });
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 702, "Litigant in person", 5, 203 },
                    { 703, "Representative", 6, 203 },
                    { 704, "Witness", 5, 203 },
                    { 705, "Litigant in person", 5, 204 },
                    { 706, "Representative", 6, 204 },
                    { 707, "Witness", 5, 204 },
                    { 708, "Judge", 4, 205 },
                    { 709, "Panel Member", 5, 206 },
                    { 710, "Observer", 5, 207 },
                    { 711, "Litigant in person", 5, 208 },
                    { 712, "Representative", 6, 208 },
                    { 713, "Witness", 5, 208 },
                 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 702; hearingRoleID <= 710; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
            for (int CaseRoleRoleID = 203; CaseRoleRoleID <= 208; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }
            for (int HearingTypeId = 175; HearingTypeId <= 177; HearingTypeId++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", HearingTypeId);
            }
            migrationBuilder.DeleteData("CaseType", "Id", 38);
        }
    }
}
