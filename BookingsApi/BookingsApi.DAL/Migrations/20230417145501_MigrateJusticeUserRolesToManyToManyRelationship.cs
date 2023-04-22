using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class MigrateJusticeUserRolesToManyToManyRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO JusticeUserRoles (JusticeUserId, UserRoleId) SELECT JusticeUser.Id, JusticeUser.UserRoleId FROM JusticeUser;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE Table JusticeUserRoles");

        }
    }
}
