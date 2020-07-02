using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;

namespace Bookings.DAL.Migrations
{
    public partial class add_observer_panelmember_roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {49, "Observer", (int) CaseRoleGroup.Observer, 1},
                    {50, "Panel Member", (int) CaseRoleGroup.PanelMember, 1},
                    {51, "Observer", (int) CaseRoleGroup.Observer, 2},
                    {52, "Panel Member", (int) CaseRoleGroup.PanelMember, 2},
                    {53, "Observer", (int) CaseRoleGroup.Observer, 3},
                    {54, "Panel Member", (int) CaseRoleGroup.PanelMember, 3},
                    {55, "Observer", (int) CaseRoleGroup.Observer, 4},
                    {56, "Panel Member", (int) CaseRoleGroup.PanelMember, 4},
                    {57, "Observer", (int) CaseRoleGroup.Observer, 5},
                    {58, "Panel Member", (int) CaseRoleGroup.PanelMember, 5},
                    {59, "Observer", (int) CaseRoleGroup.Observer, 6},
                    {60, "Panel Member", (int) CaseRoleGroup.PanelMember, 6},
                    {61, "Observer", (int) CaseRoleGroup.Observer, 7},
                    {62, "Panel Member", (int) CaseRoleGroup.PanelMember, 7},
                    {63, "Observer", (int) CaseRoleGroup.Observer, 8},
                    {64, "Panel Member", (int) CaseRoleGroup.PanelMember, 8},
                    {65, "Observer", (int) CaseRoleGroup.Observer, 9},
                    {66, "Panel Member", (int) CaseRoleGroup.PanelMember, 9},
                    {67, "Observer", (int) CaseRoleGroup.Observer, 10},
                    {68, "Panel Member", (int) CaseRoleGroup.PanelMember, 10},
                    {69, "Observer", (int) CaseRoleGroup.Observer, 11},
                    {70, "Panel Member", (int) CaseRoleGroup.PanelMember, 11},
                    {71, "Observer", (int) CaseRoleGroup.Observer, 12},
                    {72, "Panel Member", (int) CaseRoleGroup.PanelMember, 12},
                    {73, "Observer", (int) CaseRoleGroup.Observer, 13},
                    {74, "Panel Member", (int) CaseRoleGroup.PanelMember, 13},
                    {75, "Observer", (int) CaseRoleGroup.Observer, 14},
                    {76, "Panel Member", (int) CaseRoleGroup.PanelMember, 14},
                    {77, "Observer", (int) CaseRoleGroup.Observer, 15},
                    {78, "Panel Member", (int) CaseRoleGroup.PanelMember, 15},
                    {79, "Observer", (int) CaseRoleGroup.Observer, 16},
                    {80, "Panel Member", (int) CaseRoleGroup.PanelMember, 16},
                });

            migrationBuilder.InsertData(
                table: nameof(HearingRole),
                columns: new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                values: new object[,]
                {
                    { 85, "Observer", 5, 49 },
                    { 86, "Panel Member", 5, 50 },
                    { 87, "Observer", 5, 51 },
                    { 88, "Panel Member", 5, 52 },
                    { 89, "Observer", 5, 53 },
                    { 90, "Panel Member", 5, 54 },
                    { 91, "Observer", 5, 55 },
                    { 92, "Panel Member", 5, 56 },
                    { 93, "Observer", 5, 57 },
                    { 94, "Panel Member", 5, 58 },
                    { 95, "Observer", 5, 59 },
                    { 96, "Panel Member", 5, 60 },
                    { 97, "Observer", 5, 61 },
                    { 98, "Panel Member", 5, 62 },
                    { 99, "Observer", 5, 63 },
                    { 100, "Panel Member", 5, 64 },
                    { 101, "Observer", 5, 65 },
                    { 102, "Panel Member", 5, 66 },
                    { 103, "Observer", 5, 67 },
                    { 104, "Panel Member", 5, 68 },
                    { 105, "Observer", 5, 69 },
                    { 106, "Panel Member", 5, 70 },
                    { 107, "Observer", 5, 71 },
                    { 108, "Panel Member", 5, 72 },
                    { 109, "Observer", 5, 73 },
                    { 110, "Panel Member", 5, 74 },
                    { 111, "Observer", 5, 75 },
                    { 112, "Panel Member", 5, 76 },
                    { 113, "Observer", 5, 77 },
                    { 114, "Panel Member", 5, 78 },
                    { 115, "Observer", 5, 79 },
                    { 116, "Panel Member", 5, 80 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 85);
            migrationBuilder.DeleteData("HearingRole", "Id", 86);
            migrationBuilder.DeleteData("HearingRole", "Id", 87);
            migrationBuilder.DeleteData("HearingRole", "Id", 88);
            migrationBuilder.DeleteData("HearingRole", "Id", 89);
            migrationBuilder.DeleteData("HearingRole", "Id", 90);
            migrationBuilder.DeleteData("HearingRole", "Id", 91);
            migrationBuilder.DeleteData("HearingRole", "Id", 92);
            migrationBuilder.DeleteData("HearingRole", "Id", 93);
            migrationBuilder.DeleteData("HearingRole", "Id", 94);
            migrationBuilder.DeleteData("HearingRole", "Id", 95);
            migrationBuilder.DeleteData("HearingRole", "Id", 96);
            migrationBuilder.DeleteData("HearingRole", "Id", 97);
            migrationBuilder.DeleteData("HearingRole", "Id", 98);
            migrationBuilder.DeleteData("HearingRole", "Id", 99);
            migrationBuilder.DeleteData("HearingRole", "Id", 100);
            migrationBuilder.DeleteData("HearingRole", "Id", 101);
            migrationBuilder.DeleteData("HearingRole", "Id", 102);
            migrationBuilder.DeleteData("HearingRole", "Id", 103);
            migrationBuilder.DeleteData("HearingRole", "Id", 104);
            migrationBuilder.DeleteData("HearingRole", "Id", 105);
            migrationBuilder.DeleteData("HearingRole", "Id", 106);
            migrationBuilder.DeleteData("HearingRole", "Id", 107);
            migrationBuilder.DeleteData("HearingRole", "Id", 108);
            migrationBuilder.DeleteData("HearingRole", "Id", 109);
            migrationBuilder.DeleteData("HearingRole", "Id", 110);
            migrationBuilder.DeleteData("HearingRole", "Id", 111);
            migrationBuilder.DeleteData("HearingRole", "Id", 112);
            migrationBuilder.DeleteData("HearingRole", "Id", 113);
            migrationBuilder.DeleteData("HearingRole", "Id", 114);
            migrationBuilder.DeleteData("HearingRole", "Id", 115);
            migrationBuilder.DeleteData("HearingRole", "Id", 116);

            migrationBuilder.DeleteData("CaseRole", "Id", 49);
            migrationBuilder.DeleteData("CaseRole", "Id", 50);
            migrationBuilder.DeleteData("CaseRole", "Id", 51);
            migrationBuilder.DeleteData("CaseRole", "Id", 52);
            migrationBuilder.DeleteData("CaseRole", "Id", 53);
            migrationBuilder.DeleteData("CaseRole", "Id", 54);
            migrationBuilder.DeleteData("CaseRole", "Id", 55);
            migrationBuilder.DeleteData("CaseRole", "Id", 56);
            migrationBuilder.DeleteData("CaseRole", "Id", 57);
            migrationBuilder.DeleteData("CaseRole", "Id", 58);
            migrationBuilder.DeleteData("CaseRole", "Id", 59);
            migrationBuilder.DeleteData("CaseRole", "Id", 60);
            migrationBuilder.DeleteData("CaseRole", "Id", 61);
            migrationBuilder.DeleteData("CaseRole", "Id", 62);
            migrationBuilder.DeleteData("CaseRole", "Id", 63);
            migrationBuilder.DeleteData("CaseRole", "Id", 64);
            migrationBuilder.DeleteData("CaseRole", "Id", 65);
            migrationBuilder.DeleteData("CaseRole", "Id", 66);
            migrationBuilder.DeleteData("CaseRole", "Id", 67);
            migrationBuilder.DeleteData("CaseRole", "Id", 68);
            migrationBuilder.DeleteData("CaseRole", "Id", 69);
            migrationBuilder.DeleteData("CaseRole", "Id", 70);
            migrationBuilder.DeleteData("CaseRole", "Id", 71);
            migrationBuilder.DeleteData("CaseRole", "Id", 72);
            migrationBuilder.DeleteData("CaseRole", "Id", 73);
            migrationBuilder.DeleteData("CaseRole", "Id", 74);
            migrationBuilder.DeleteData("CaseRole", "Id", 75);
            migrationBuilder.DeleteData("CaseRole", "Id", 76);
            migrationBuilder.DeleteData("CaseRole", "Id", 77);
            migrationBuilder.DeleteData("CaseRole", "Id", 78);
            migrationBuilder.DeleteData("CaseRole", "Id", 79);
            migrationBuilder.DeleteData("CaseRole", "Id", 80);

        }
    }
}
