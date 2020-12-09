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
                    { 159, "Appellant", (int) CaseRoleGroup.Appellant, 19 },
                    { 160, "Appellant", (int) CaseRoleGroup.Appellant, 20 },
                    { 161, "Appellant", (int) CaseRoleGroup.Appellant, 21 },
                    { 162, "Appellant", (int) CaseRoleGroup.Appellant, 22 },
                    { 163, "Appellant", (int) CaseRoleGroup.Appellant, 23 },
                    { 164, "Appellant", (int) CaseRoleGroup.Appellant, 24 },
                    { 165, "Appellant", (int) CaseRoleGroup.Appellant, 25 },
                    { 166, "Appellant", (int) CaseRoleGroup.Appellant, 26 },
                    { 167, "Appellant", (int) CaseRoleGroup.Appellant, 27 },
                    { 168, "Appellant", (int) CaseRoleGroup.Appellant, 28 },
                    { 169, "Appellant", (int) CaseRoleGroup.Appellant, 29 },
                    { 170, "Appellant", (int) CaseRoleGroup.Appellant, 30 },
                    { 171, "Appellant", (int) CaseRoleGroup.Appellant, 31 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    { 338, "Litigant in person", 5, 159 },
                    { 339, "Representative", 6, 159 },
                    { 340, "Witness", 5, 159 },
                    { 341, "Litigant in person", 5, 160 },
                    { 342, "Representative", 6, 160 },
                    { 343, "Witness", 5, 160 },
                    { 344, "Litigant in person", 5, 161 },
                    { 345, "Representative", 6, 161 },
                    { 346, "Witness", 5, 161 },
                    { 347, "Litigant in person", 5, 162 },
                    { 348, "Representative", 6, 162 },
                    { 349, "Witness", 5, 162 },
                    { 350, "Litigant in person", 5, 163 },
                    { 351, "Representative", 6, 163 },
                    { 352, "Witness", 5, 163 },
                    { 353, "Litigant in person", 5, 164 },
                    { 354, "Representative", 6, 164 },
                    { 355, "Witness", 5, 164 },
                    { 356, "Litigant in person", 5, 165 },
                    { 357, "Representative", 6, 165 },
                    { 358, "Witness", 5, 165 },
                    { 359, "Litigant in person", 5, 166 },
                    { 360, "Representative", 6, 166 },
                    { 361, "Witness", 5, 166 },
                    { 362, "Litigant in person", 5, 167 },
                    { 363, "Representative", 6, 167 },
                    { 364, "Witness", 5, 167 },
                    { 365, "Litigant in person", 5, 168 },
                    { 366, "Representative", 6, 168 },
                    { 367, "Witness", 5, 168 },
                    { 368, "Litigant in person", 5, 169 },
                    { 369, "Representative", 6, 169 },
                    { 370, "Witness", 5, 169 },
                    { 371, "Litigant in person", 5, 170 },
                    { 372, "Representative", 6, 170 },
                    { 373, "Witness", 5, 170 },
                    { 374, "Litigant in person", 5, 171 },
                    { 375, "Representative", 6, 171 },
                    { 376, "Witness", 5, 171 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 338; hearingRoleID <= 376; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
            for (int CaseRoleRoleID = 159; CaseRoleRoleID <= 171; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }
        }
    }
}
