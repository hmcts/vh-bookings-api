using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class RenameSolicitorToRepresentative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update HearingRole Set Name = 'Representative' Where Name = 'Solicitor'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update HearingRole Set Name = 'Solicitor' Where Name = 'Representative'");
        }
    }
}
