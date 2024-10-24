using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    [DbContext(typeof(RefDataContext))]
    [Migration("20241015150655_AddEnglishInterpreterLanguage")]
    public partial class AddEnglishInterpreterLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/10825_addenglish_interpreter_languages.sql", migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
