using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class FixForSENDCaseTypeUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = $@"
                declare @caseTypeId int;
                select @caseTypeId = Id from CaseType
                where Name = 'Special Educational Needs and Disability';

                declare @caseRoleId1 int;
                select @caseRoleId1 = Id from CaseRole
                where CaseTypeId = @caseTypeId and Name = 'Appellant';

                declare @caseRoleId2 int;
                select @caseRoleId2 = Id from CaseRole
                where CaseTypeId = @caseTypeId and Name = 'Local Authority'

                declare @hearingRoleId1 int;
                select @hearingRoleId1 = Id from HearingRole
                where Name = 'MacKenzie Friend' and CaseRoleId = @caseRoleId1

                declare @hearingRoleId2 int;
                select @hearingRoleId2 = Id from HearingRole
                where Name = 'Witness' and CaseRoleId = @caseRoleId1

                declare @hearingRoleId3 int;
                select @hearingRoleId3 = Id from HearingRole
                where Name = 'Witness' and CaseRoleId = @caseRoleId2

                declare @userRoleId int;
                select @userRoleId = Id from UserRole where Name = 'Individual';

                update HearingRole set UserRoleId = @userRoleId
                where Id in (@hearingRoleId1, @hearingRoleId2, @hearingRoleId3);
                ";

            migrationBuilder.Sql(sqlScript);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"
                declare @caseTypeId int
                -- get CaseTypeId
                select @caseTypeId = Id from CaseType
                where Name = 'Special Educational Needs and Disability'

                declare @caseRoleIds table
                (
                    Value int
                )
                -- get CaseRoleIds
                insert into @caseRoleIds select Id from CaseRole where CaseTypeId = @caseTypeId
                -- delete all HearingRoles connected to CaseRoleIds
                delete from HearingRole where CaseRoleId in (select value from @caseRoleIds)

                -- delete all HearingType connected to CaseTypeId
                delete from HearingType where CaseTypeId = @caseTypeId

                -- delete all CaseRole connected to CaseTypeId
                delete from CaseRole where CaseTypeId = @caseTypeId

                DELETE FROM CaseType WHERE Id = @caseTypeId
                ";
            
            migrationBuilder.Sql(sqlScript);
        }
    }
}
