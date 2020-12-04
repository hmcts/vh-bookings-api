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
            migrationBuilder.Sql(@"UPDATE Participant SET Discriminator = 'Individual' 
                WHERE Id = 'bb4a09d2-447e-4669-a0db-bec363419f34'");
        }
    }
}
