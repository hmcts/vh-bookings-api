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

                declare @caseRoleId3 int;
                select @caseRoleId3 = Id from CaseRole
                where CaseTypeId = @caseTypeId and Name = 'Observer'

                declare @hearingRoleId1 int;
                select @hearingRoleId1 = Id from HearingRole
                where Name = 'MacKenzie Friend' and CaseRoleId = @caseRoleId1

                declare @hearingRoleId2 int;
                select @hearingRoleId2 = Id from HearingRole
                where Name = 'Witness' and CaseRoleId = @caseRoleId1

                declare @hearingRoleId3 int;
                select @hearingRoleId3 = Id from HearingRole
                where Name = 'Witness' and CaseRoleId = @caseRoleId2

                declare @hearingRoleId4 int;
                select @hearingRoleId4 = Id from HearingRole
                where Name = 'Observer' and CaseRoleId = @caseRoleId3

                declare @userRoleId int;
                select @userRoleId = Id from UserRole where Name = 'Individual';

                update HearingRole set UserRoleId = @userRoleId
                where Id in (@hearingRoleId1, @hearingRoleId2, @hearingRoleId3, @hearingRoleId4);
                ";

            migrationBuilder.Sql(sqlScript);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

                declare @caseRoleId3 int;
                select @caseRoleId3 = Id from CaseRole
                where CaseTypeId = @caseTypeId and Name = 'Observer'

                declare @hearingRoleId1 int;
                select @hearingRoleId1 = Id from HearingRole
                where Name = 'MacKenzie Friend' and CaseRoleId = @caseRoleId1

                declare @hearingRoleId2 int;
                select @hearingRoleId2 = Id from HearingRole
                where Name = 'Witness' and CaseRoleId = @caseRoleId1

                declare @hearingRoleId3 int;
                select @hearingRoleId3 = Id from HearingRole
                where Name = 'Witness' and CaseRoleId = @caseRoleId2

                declare @hearingRoleId4 int;
                select @hearingRoleId4 = Id from HearingRole
                where Name = 'Observer' and CaseRoleId = @caseRoleId3

                declare @userRoleId int;
                select @userRoleId = Id from UserRole where Name = 'Representative';

                update HearingRole set UserRoleId = @userRoleId
                where Id in (@hearingRoleId1, @hearingRoleId2, @hearingRoleId3, @hearingRoleId4);
                ";
            
            migrationBuilder.Sql(sqlScript);
        }
    }
}
