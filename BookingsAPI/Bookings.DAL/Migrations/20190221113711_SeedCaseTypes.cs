using Bookings.DAL.SeedData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class SeedCaseTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            new SeedCaseTypesData().Up(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            new SeedCaseTypesData().Down(migrationBuilder);
        }
    }
}
