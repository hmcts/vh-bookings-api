using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class add_case_role_apellant_state : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: nameof(CaseRole),
              columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
              values: new object[,]
              {
                    {81, "Appellant", (int) CaseRoleGroup.Appellant, 5},
                    {82, "State", (int) CaseRoleGroup.State, 5},

              });

            migrationBuilder.InsertData(
             table: nameof(HearingRole),
             columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
             values: new object[,]
             {
                    { 117, "Appellant LIP", 5, 81 },
                    { 118, "Representative", 6, 81 },
                    { 119, "State LIP", 5, 82 },
                    { 120, "Representative", 6, 82 }
             });
   }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 117);
            migrationBuilder.DeleteData("HearingRole", "Id", 118);
            migrationBuilder.DeleteData("HearingRole", "Id", 119);
            migrationBuilder.DeleteData("HearingRole", "Id", 120);

            migrationBuilder.DeleteData("CaseRole", "Id", 81);
            migrationBuilder.DeleteData("CaseRole", "Id", 82);
        }
    }
}
