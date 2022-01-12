using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseAndRoleTypeForUpperTaxTribunal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name" },
                new object[,]
                {
                    {39, "Upper Tribunal Tax"}
                });

            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    {178, "Permission to Appeal", 39}
                });

            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId" },
                new object[,]
                {
                    { 251, "Appellant", (int) CaseRoleGroup.Appellant, 39 },
                    { 252, "Respondent", (int) CaseRoleGroup.Respondent, 39 },
                    { 253, "Panel Member", (int) CaseRoleGroup.PanelMember, 39 },
                    { 254, "Observer", (int) CaseRoleGroup.Observer, 39 },
                    { 260, "Judge", (int) CaseRoleGroup.Judge, 39 }
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Appellant(251)
                    {759, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 251},
                    {760, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 251},
                    {761, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 251},
                    {762, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 251},
                    // Respondent(252)
                    {763, HearingRoles.LitigantInPerson, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 252},
                    {764, HearingRoles.Interpreter, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 252},
                    {765, HearingRoles.Representative, UserRoleForHearingRole.UserRoleId[UserRoles.Representative], 252},
                    {766, HearingRoles.Witness, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 252},
                    // Panel Member(253)
                    {767, HearingRoles.PanelMember, UserRoleForHearingRole.UserRoleId[UserRoles.PanelMember], 253},
                    // Observer(254)
                    {768, HearingRoles.Observer, UserRoleForHearingRole.UserRoleId[UserRoles.Individual], 254},

                    //Judge
                    {791, HearingRoles.Judge, UserRoleForHearingRole.UserRoleId[UserRoles.Judge], 260},
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 759; hearingRoleId <= 768; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(HearingRole), "Id", hearingRoleId);
            }

            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 791);

            for (int hearingRoleId = 251; hearingRoleId <= 254; hearingRoleId++)
            {
                migrationBuilder.DeleteData(nameof(CaseRole), "Id", hearingRoleId);
            }

            migrationBuilder.DeleteData(nameof(CaseRole), "Id", 260);

            migrationBuilder.DeleteData(nameof(CaseType), "Id", 39);
        }
    }
}
