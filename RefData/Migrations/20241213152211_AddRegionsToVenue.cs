using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionsToVenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/10965_add_region_to_venue.sql", migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
