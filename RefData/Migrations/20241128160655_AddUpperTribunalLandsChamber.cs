using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    [DbContext(typeof(RefDataContext))]
    [Migration("20241128160655_AddUpperTribunalLandsChamber")]
    public partial class AddUpperTribunalLandsChamber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/11173_mrd_add_case_types_Upper_Tribunal_Lands_Chamber.sql", migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}


