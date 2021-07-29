using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddPanelMemberRoleToFamilyCaseTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Panel Member UserId is incorrect updated in UpdatePanelMemberUserRoleId migration

             migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 209, "Panel Member", (int) CaseRoleGroup.PanelMember, 33 },
                    { 210, "Panel Member", (int) CaseRoleGroup.PanelMember, 34 },
                    { 211, "Panel Member", (int) CaseRoleGroup.PanelMember, 35 },
                    { 212, "Panel Member", (int) CaseRoleGroup.PanelMember, 37 },
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {       
                    { 714, "Panel Member", 5, 209 },
                    { 715, "Panel Member", 5, 210 },
                    { 716, "Panel Member", 5, 211 },
                    { 717, "Panel Member", 5, 212 },
                 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleID = 714; hearingRoleID <= 717; hearingRoleID++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", hearingRoleID);
            }
            for (int CaseRoleRoleID = 209; CaseRoleRoleID <= 212; CaseRoleRoleID++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", CaseRoleRoleID);
            }
        }
    }
}
