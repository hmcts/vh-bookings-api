using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddAppellantCaseRoleForGeneralRegulatoryChamberTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 160, "Appellant", (int) CaseRoleGroup.Appellant, 19 },
                    { 161, "Appellant", (int) CaseRoleGroup.Appellant, 20 },
                    { 162, "Appellant", (int) CaseRoleGroup.Appellant, 21 },
                    { 163, "Appellant", (int) CaseRoleGroup.Appellant, 22 },
                    { 164, "Appellant", (int) CaseRoleGroup.Appellant, 23 },
                    { 165, "Appellant", (int) CaseRoleGroup.Appellant, 24 },
                    { 166, "Appellant", (int) CaseRoleGroup.Appellant, 25 },
                    { 167, "Appellant", (int) CaseRoleGroup.Appellant, 26 },
                    { 168, "Appellant", (int) CaseRoleGroup.Appellant, 27 },
                    { 169, "Appellant", (int) CaseRoleGroup.Appellant, 28 },
                    { 170, "Appellant", (int) CaseRoleGroup.Appellant, 29 },
                    { 171, "Appellant", (int) CaseRoleGroup.Appellant, 30 },
                    { 172, "Appellant", (int) CaseRoleGroup.Appellant, 31 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "Live" },
                new object[,]
                {
                    { 339, "Litigant in person", 5, 160, true },
                    { 340, "Representative", 6, 160, true },
                    { 341, "Witness", 5, 160, true },
                    { 342, "Litigant in person", 5, 161, true },
                    { 343, "Representative", 6, 161, true },
                    { 344, "Witness", 5, 161, true },
                    { 345, "Litigant in person", 5, 162, true },
                    { 346, "Representative", 6, 162, true },
                    { 347, "Witness", 5, 162, true },
                    { 348, "Litigant in person", 5, 163, true },
                    { 349, "Representative", 6, 163, true },
                    { 350, "Witness", 5, 163, true },
                    { 351, "Litigant in person", 5, 164, true },
                    { 352, "Representative", 6, 164, true },
                    { 353, "Witness", 5, 164, true },
                    { 354, "Litigant in person", 5, 165, true },
                    { 355, "Representative", 6, 165, true },
                    { 356, "Witness", 5, 165, true },
                    { 357, "Litigant in person", 5, 166, true },
                    { 358, "Representative", 6, 166, true },
                    { 359, "Witness", 5, 166, true },
                    { 360, "Litigant in person", 5, 167, true },
                    { 361, "Representative", 6, 167, true },
                    { 362, "Witness", 5, 167, true },
                    { 363, "Litigant in person", 5, 168, true },
                    { 364, "Representative", 6, 168, true },
                    { 365, "Witness", 5, 168, true },
                    { 366, "Litigant in person", 5, 169, true },
                    { 367, "Representative", 6, 169, true },
                    { 368, "Witness", 5, 169, true },
                    { 369, "Litigant in person", 5, 170, true },
                    { 370, "Representative", 6, 170, true },
                    { 371, "Witness", 5, 170, true },
                    { 372, "Litigant in person", 5, 171, true },
                    { 373, "Representative", 6, 171, true },
                    { 374, "Witness", 5, 171, true },
                    { 375, "Litigant in person", 5, 172, true },
                    { 376, "Representative", 6, 172, true },
                    { 377, "Witness", 5, 172, true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 339; hearingRoleID <= 377; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
            for (int CaseRoleRoleID = 160; CaseRoleRoleID <= 172; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }
        }
    }
}
