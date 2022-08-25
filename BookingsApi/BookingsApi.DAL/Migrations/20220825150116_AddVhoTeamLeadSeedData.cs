using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddVhoTeamLeadSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var vhoTeamLeadUserRoleId = 9;

            migrationBuilder.InsertData(
                table: nameof(UserRole),
                columns: new[] {
                    "Id",
                    "Name",
                },
                values: new object[,]
                {
                    {
                        vhoTeamLeadUserRoleId,
                        "Video hearings team lead"
                    }
                }
           );
            
            migrationBuilder.InsertData(
                table: nameof(JusticeUser),
                columns: new[] {
                    "Id",
                    "CreatedDate",
                    "FirstName",
                    "Lastname",
                    "ContactEmail",
                    "Username",
                    "UserRole",
                    "CreatedBy",
                },
                values: new object[,]
                {
                    {
                        "ba3fc1e6-6f4d-4ad4-b4a9-8e7e70d60099",
                        "2022-08-25 09:00",
                        "Manual",
                        "Vhoteamlead1",
                        "manual.vhoteamlead1@hearings.reform.hmcts.net",
                        "manual.vhoteamlead1@hearings.reform.hmcts.net",
                        vhoTeamLeadUserRoleId,
                        "Arif.Ahmed@hearings.reform.hmcts.net",
                    },
                    {
                        "5fa2274d-5604-4d02-9f14-024ba17c1e3a",
                        "2022-08-25 09:00",
                        "Manual",
                        "Vhoteamlead2",
                        "manual.vhoteamlead2@hearings.reform.hmcts.net",
                        "manual.vhoteamlead2@hearings.reform.hmcts.net",
                        vhoTeamLeadUserRoleId,
                        "Arif.Ahmed@hearings.reform.hmcts.net",
                    },
                    {
                        "32152059-22be-45ef-88d9-315f01f8d118",
                        "2022-08-25 09:00",
                        "Auto",
                        "Vhoteamlead1",
                        "auto.vhoteamlead1@hearings.reform.hmcts.net",
                        "auto.vhoteamlead1@hearings.reform.hmcts.net",
                        vhoTeamLeadUserRoleId,
                        "Arif.Ahmed@hearings.reform.hmcts.net",
                    },
                    {
                        "f0369900-ede8-4989-8e57-0daf4cc8e3fb",
                        "2022-08-25 09:00",
                        "Auto",
                        "Vhoteamlead2",
                        "auto.vhoteamlead2@hearings.reform.hmcts.net",
                        "auto.vhoteamlead2@hearings.reform.hmcts.net",
                        vhoTeamLeadUserRoleId,
                        "Arif.Ahmed@hearings.reform.hmcts.net",
                    },
                }
           );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("UserRole", "Id", 9);
            migrationBuilder.DeleteData("JusticeUser", "Id", "ba3fc1e6-6f4d-4ad4-b4a9-8e7e70d60099");
            migrationBuilder.DeleteData("JusticeUser", "Id", "5fa2274d-5604-4d02-9f14-024ba17c1e3a");
            migrationBuilder.DeleteData("JusticeUser", "Id", "32152059-22be-45ef-88d9-315f01f8d118");
            migrationBuilder.DeleteData("JusticeUser", "Id", "f0369900-ede8-4989-8e57-0daf4cc8e3fb");
        }
    }
}
