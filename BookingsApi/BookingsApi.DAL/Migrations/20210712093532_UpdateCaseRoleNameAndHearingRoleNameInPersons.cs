using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateCaseRoleNameAndHearingRoleNameInPersons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Person] SET [CaseRoleName] = 'Judge', [HearingRoleName] = 'Judge' FROM [dbo].[Person] INNER JOIN [dbo].[JudiciaryPerson] ON [dbo].[Person].Username =  [dbo].[JudiciaryPerson].Email OR [dbo].[Person].ContactEmail =  [dbo].[JudiciaryPerson].Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Person] SET [CaseRoleName] = 'NULL', [HearingRoleName] = 'NULL' FROM [dbo].[Person] INNER JOIN [dbo].[JudiciaryPerson] ON [dbo].[Person].Username =  [dbo].[JudiciaryPerson].Email OR [dbo].[Person].ContactEmail =  [dbo].[JudiciaryPerson].Email");
        }
    }
}
