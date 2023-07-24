using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RefData.Migrations;

public static class SqlFileHelper
{
    public static void RunSqlFile(string filePath, MigrationBuilder migrationBuilder)
    {
        var sql = File.ReadAllText(filePath);
        var batches = sql.Split(new [] {"GO;"}, StringSplitOptions.None);
        foreach (string batch in batches)
        {
            migrationBuilder.Sql(batch);
        }
    }
}