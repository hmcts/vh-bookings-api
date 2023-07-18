using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class HearingVenueChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine("Running 9777_rename_venues_and_add_venue_codes");
            var sqlFile1 = Path.Combine("data/9777_rename_venues_and_add_venue_codes.sql");
            RunSqlFile(File.ReadAllText(sqlFile1), migrationBuilder);
            Console.WriteLine("Completed 9777_rename_venues_and_add_venue_codes");
            // migrationBuilder.Sql(File.ReadAllText(sqlFile1));

            Console.WriteLine("Running 9965_add_private_law_case_type");
            var sqlFile2 = Path.Combine("data/9965_add_private_law_case_type.sql");
            // migrationBuilder.Sql(File.ReadAllText(sqlFile2));
            RunSqlFile(File.ReadAllText(sqlFile2), migrationBuilder);
            Console.WriteLine("Complete 9965_add_private_law_case_type");

            Console.WriteLine("Running 9987_add_venue_codes");
            var sqlFile3 = Path.Combine("data/9987_add_venue_codes.sql");
            // migrationBuilder.Sql(File.ReadAllText(sqlFile3));
            RunSqlFile(File.ReadAllText(sqlFile3), migrationBuilder);
            Console.WriteLine("Completed 9987_add_venue_codes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private void RunSqlFile(string sql, MigrationBuilder migrationBuilder)
        {
            string[] batches = sql.Split(new [] {"GO;"}, StringSplitOptions.None);
            foreach (string batch in batches)
            {
                Console.WriteLine(batch);
                migrationBuilder.Sql(batch);
            }
        }
    }
}
