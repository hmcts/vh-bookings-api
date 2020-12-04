using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookings.DAL.Migrations
{
    public partial class SetParticipantDiscriminatorToJudicialOfficeHolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE Participant
                SET Discriminator = 'JudicialOfficeHolder'
                FROM Participant p 
                INNER JOIN HearingRole hr ON p.HearingRoleId = hr.Id
                WHERE hr.Name = 'Panel Member' OR hr.Name = 'Winger'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Method left empty intentionally
        }
    }
}
