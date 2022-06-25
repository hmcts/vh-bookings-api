using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddJudiciaryPersonStaging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JudiciaryPersonsStaging",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExternalRefId = table.Column<string>(nullable: true),
                    PersonalCode = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    KnownAs = table.Column<string>(nullable: true),
                    Fullname = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    PostNominals = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Leaver = table.Column<string>(nullable: true),
                    LeftOn = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudiciaryPersonsStaging", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JudiciaryPersonsStaging");
        }
    }
}
