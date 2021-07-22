using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateAccountTypeForCourtRoomAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var judges = UserApiWrapperClient.GetJudgesFromAdAsync().GetAwaiter().GetResult();

            foreach(var judge in judges)
            {
               
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
