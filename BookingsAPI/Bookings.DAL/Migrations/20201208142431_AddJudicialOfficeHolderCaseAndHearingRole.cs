﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class AddJudicialOfficeHolderCaseAndHearingRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                "CaseRole",
                new[] {"Id", "Name", "Group", "CaseTypeId"},
                new object[,]
                {
                    {159, "Judicial Office Holder", 11, 1}
                });

            migrationBuilder.InsertData(
                "HearingRole",
                new [] { "Id" ,"Name", "UserRoleId", "CaseRoleId", "Live" },
                new object[,]
                {
                    {338, "Judicial Office Holder", 7, 159, false}
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("HearingRole", "Id", 338);
            migrationBuilder.DeleteData("CaseRole", "Id", 159);
        }
    }
}
