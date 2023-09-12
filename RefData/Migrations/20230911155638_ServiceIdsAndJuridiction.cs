using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class ServiceIdsAndJuridiction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/9965_add_private_law_case_type.sql", migrationBuilder);
            SqlFileHelper.RunSqlFile("data/10051_add_service_ids_and_jurisidictions.sql", migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
