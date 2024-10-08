using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    [DbContext(typeof(RefDataContext))]
    [Migration("20241004070055_AddInterpreterLanguagesWelsh")]
    public partial class AddInterpreterLanguagesWelsh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/10837_update_interpreter_languages_welsh.sql", migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
