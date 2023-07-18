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
            var sqlFile1 = Path.Combine("data/9777_rename_venues_and_add_venue_codes.sql");
            RunSqlFile(File.ReadAllText(sqlFile1), migrationBuilder);
            
            var sqlFile2 = Path.Combine("data/9965_add_private_law_case_type.sql");
            RunSqlFile(File.ReadAllText(sqlFile2), migrationBuilder);
            
            var sqlFile3 = Path.Combine("data/9987_add_venue_codes.sql");
            RunSqlFile(File.ReadAllText(sqlFile3), migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        /// <summary>
        /// EF Core does not like GO statements in the migration files, so we need to split the file into batches
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="migrationBuilder"></param>
        private void RunSqlFile(string sql, MigrationBuilder migrationBuilder)
        {
            string[] batches = sql.Split(new [] {"GO;"}, StringSplitOptions.None);
            foreach (string batch in batches)
            {
                migrationBuilder.Sql(batch);
            }
        }
    }
}
