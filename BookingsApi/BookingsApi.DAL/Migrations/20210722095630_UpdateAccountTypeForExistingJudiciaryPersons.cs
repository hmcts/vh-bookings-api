using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateAccountTypeForExistingJudiciaryPersons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Person] SET [AccountType] = 'Judiciary' FROM [dbo].[Person] INNER JOIN [dbo].[JudiciaryPerson] ON [dbo].[Person].Username = [dbo].[JudiciaryPerson].Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Person] SET [AccountType] = NULL FROM [dbo].[Person] INNER JOIN [dbo].[JudiciaryPerson] ON [dbo].[Person].Username = [dbo].[JudiciaryPerson].Email");
        }
    }
}
