using BookingsApi.Contract.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddSENDCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "CaseType",
                new[] { "Id", "Name" },
                new object[,]
                {
                    {51, "Special Educational Needs and Disability"}
                });
            
            migrationBuilder.InsertData(
                "HearingType",
                new[] { "Id", "Name", "CaseTypeId", "Live"},
                new object[,]
                {
                    {274, "Case Management Hearing",51,true},
                    {275, "Final Hearing",51,true}
                });
            migrationBuilder.InsertData(
                "CaseRole",
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 320, "Appellant", (int) CaseRoleGroup.Appellant, 51 },
                    { 321, "Local Authority", (int) CaseRoleGroup.LocalAuthority, 51 },
                    { 322, "Observer", (int) CaseRoleGroup.Observer, 51 },
                    { 323, "Panel Member", (int) CaseRoleGroup.PanelMember, 51 },
                    { 324, "Judge",  (int) CaseRoleGroup.Judge, 51 }
                });

            migrationBuilder.InsertData(
                "HearingRole",
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "Live"},
                new object[,]
                {
                    { 1013, "Appellant", 5, 320,true },
                    { 1014, "Expert", 5, 320,true },
                    { 1015, "Intermediary", 5, 320,true },
                    { 1016, "Interpreter", 5, 320,true },
                    { 1017, "Litigant In Person", 5, 320,true },
                    { 1018, "Litigation Friend", 5, 320,true },
                    { 1019, "MacKenzie Friend", 5, 320,true },
                    { 1020, "Representative", 6, 320,true },
                    { 1021, "Solicitor", 6, 320,true },
                    { 1022, "Witness", 5, 320,true },
                    { 1023, "Representative", 6, 321,true },
                    { 1024, "Witness", 5, 321,true },
                    { 1025, "Observer", 5, 322,true },
                    { 1026, "Panel Member", 7, 323,true },
                    { 1027, "Judge", 4, 324,true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
