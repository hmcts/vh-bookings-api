using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class AddGenericRefData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/9982_add_generic_ref_data.sql", migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
