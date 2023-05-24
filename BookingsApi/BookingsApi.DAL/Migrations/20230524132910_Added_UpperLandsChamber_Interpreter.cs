using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class Added_UpperLandsChamber_Interpreter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", "Live", "CreatedDate" },
                new object[,]
                {
                    { 1129, "Interpreter", (int)UserRoleId.Individual, 320, true, DateTime.UtcNow },
                    { 1130, "Interpreter", (int)UserRoleId.Individual, 321, true, DateTime.UtcNow },
                    { 1131, "Interpreter", (int)UserRoleId.Individual, 322, true, DateTime.UtcNow },
                    { 1132, "Interpreter", (int)UserRoleId.Individual, 323, true, DateTime.UtcNow },
                    { 1133, "Interpreter", (int)UserRoleId.Individual, 324, true, DateTime.UtcNow },
                    { 1134, "Interpreter", (int)UserRoleId.Individual, 325, true, DateTime.UtcNow },
                    { 1135, "Interpreter", (int)UserRoleId.Individual, 326, true, DateTime.UtcNow },
                    { 1136, "Interpreter", (int)UserRoleId.Individual, 327, true, DateTime.UtcNow },
                    { 1137, "Interpreter", (int)UserRoleId.Individual, 328, true, DateTime.UtcNow },
                    { 1138, "Interpreter", (int)UserRoleId.Individual, 329, true, DateTime.UtcNow }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1129);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1130);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1131);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1132);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1133);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1134);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1135);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1136);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1137);
            migrationBuilder.DeleteData(nameof(HearingRole), "Id", 1138);
        }
    }
}
