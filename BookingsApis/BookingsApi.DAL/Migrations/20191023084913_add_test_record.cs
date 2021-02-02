using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class add_test_record : Migration
    {
        private readonly Guid _testGuid = new Guid("93dcb4a2-d0b2-4a2f-b017-71109cc01494");

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"INSERT INTO Organisation (Name) VALUES ('{_testGuid}');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"DELETE FROM Organisation WHERE Name = '{_testGuid}'");
        }
    }
}
