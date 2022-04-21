using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class FixUserRoleForRespondentInCriminalInjuriesCompensation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"
                declare @caseTypeId int;
                -- get CaseTypeId
                select @caseTypeId = Id from CaseType
                where Name = 'Criminal Injuries Compensation';

                declare @caseRoleId int;
                select @caseRoleId = Id from CaseRole
                where CaseTypeId = @caseTypeId and Name = 'None';

                declare @hearingRoleId int;
                select @hearingRoleId = Id from HearingRole
                where Name = 'Respondent' and CaseRoleId = @caseRoleId;
                
                declare @UserRoleId1 int;
                select @UserRoleId1 = Id from UserRole where Name = 'Individual';
                
                update HearingRole set UserRoleId = @UserRoleId1  where Id = @hearingRoleId;
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
