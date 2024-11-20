using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    [DbContext(typeof(RefDataContext))]
    [Migration("20241120150655_AddUpperTribunals")]
    public partial class AddUpperTribunals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/11105_mrd_add_case_types_upper_tribunals.sql", migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
