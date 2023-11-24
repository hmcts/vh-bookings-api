using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class trimming_whitespaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update HearingType set Name = TRIM(Name) where Name like '% '");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
