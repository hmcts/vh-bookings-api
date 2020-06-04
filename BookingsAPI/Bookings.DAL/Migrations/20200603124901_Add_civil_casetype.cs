using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class Add_civil_casetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseType),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 8, "Civil" },
                });

            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 22, "Fast Track Trial", 8 }
                });

            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {22, "Claimant", (int) CaseRoleGroup.Claimant, 8},
                    {23, "Defendant", (int) CaseRoleGroup.Defendant, 8},
                    {24, "Judge", (int) CaseRoleGroup.Judge, 8},
                });

            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    { 40, "Claimant LIP", 5, 22 },
                    { 41, "Representative", 6, 22 },
                    { 42, "Defendant LIP", 5, 23 },
                    { 43, "Representative", 6, 23 },
                    { 44, "Judge", 4, 24 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 40);
            migrationBuilder.DeleteData("HearingRole", "Id", 41);
            migrationBuilder.DeleteData("HearingRole", "Id", 42);
            migrationBuilder.DeleteData("HearingRole", "Id", 43);

            migrationBuilder.DeleteData("CaseRole", "Id", 22);
            migrationBuilder.DeleteData("CaseRole", "Id", 23);
            migrationBuilder.DeleteData("CaseRole", "Id", 24);

            migrationBuilder.DeleteData("HearingType", "Id", 22);

            migrationBuilder.DeleteData("CaseType", "Id", 8);
        }
    }
}
