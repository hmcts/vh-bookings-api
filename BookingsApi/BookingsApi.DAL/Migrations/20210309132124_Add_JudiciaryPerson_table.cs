using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class Add_JudiciaryPerson_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JudiciaryPerson",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExternalRefId = table.Column<Guid>(nullable: false),
                    PersonalCode = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    KnownAs = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Fullname = table.Column<string>(nullable: true),
                    PostNominals = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudiciaryPerson", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JudiciaryPerson");
        }
    }
}
