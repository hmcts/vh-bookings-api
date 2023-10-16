using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class MRDChangesForCivilFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/10178_mrd_changes_to_civil_case_types.sql", migrationBuilder);
            SqlFileHelper.RunSqlFile("data/10181_mrd_changes_family_case_types.sql", migrationBuilder);
            SqlFileHelper.RunSqlFile("data/10192_mrd_changes_to_Tribunal_case_types.sql", migrationBuilder);
            SqlFileHelper.RunSqlFile("data/10237_mrd_add_service_id_for_generic.sql", migrationBuilder);
            SqlFileHelper.RunSqlFile("data/10194_expire_case_types_not_in_mrd.sql", migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
