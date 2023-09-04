using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class AddSingleLayerHearingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/10092_add_flat_hearing_role_structure.sql", migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
