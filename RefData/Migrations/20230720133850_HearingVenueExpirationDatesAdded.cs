using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class HearingVenueExpirationDatesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine("data/9776_add_venue_expiration_dates.sql");
            string[] batches = File.ReadAllText(sqlFile).Split(new [] {"GO;"}, StringSplitOptions.None);
            foreach (string batch in batches)
                migrationBuilder.Sql(batch);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
