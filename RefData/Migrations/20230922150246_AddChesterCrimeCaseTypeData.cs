using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class AddChesterCrimeCaseTypeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/10087_add_chester_crime_case_type.sql", migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
