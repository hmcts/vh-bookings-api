using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;




#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddCaseRoleUpperChamber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseRole),
                new[] { "Id", "Name", "Group", "CaseTypeId", "CreatedDate" },
                new object[,]
                {
                    { 350, "Interpreter", (int) CaseRoleGroup.Interpreter, 51, DateTime.UtcNow },
                    
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(nameof(CaseRole), "Id", 350);
        }
    }
}
