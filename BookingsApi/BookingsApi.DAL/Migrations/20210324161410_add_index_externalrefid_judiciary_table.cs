using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class add_index_externalrefid_judiciary_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson",
                column: "ExternalRefId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JudiciaryPerson_ExternalRefId",
                table: "JudiciaryPerson");
        }
    }
}
