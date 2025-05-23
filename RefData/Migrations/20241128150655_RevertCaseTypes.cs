﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    [DbContext(typeof(RefDataContext))]
    [Migration("20241128150655_RevertCaseTypes")]
    public partial class RevertCaseTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SqlFileHelper.RunSqlFile("data/11185_mrd_case_types_amend_names.sql", migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
