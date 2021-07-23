using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateAccountTypeForCourtRoomAccounts : Migration
    {
        private List<string> _judges;
        public UpdateAccountTypeForCourtRoomAccounts()
        {
            _judges = UserApiWrapperClient.GetJudgesFromAdAsync().GetAwaiter().GetResult();
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _judges.ForEach(judge =>
            {
                migrationBuilder.Sql($"UPDATE [dbo].[Person] SET AccountType='Court Room' where UserName={judge}");
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _judges.ForEach(judge =>
            {
                migrationBuilder.Sql($"UPDATE [dbo].[Person] SET AccountType=NULL where UserName={judge}");
            });
        }
    }
}
