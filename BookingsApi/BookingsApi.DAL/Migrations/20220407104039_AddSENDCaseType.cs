﻿using System;
using BookingsApi.Contract.Enums;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddSENDCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = $@"
            DECLARE @caseTypeId int
            INSERT INTO CaseType (""Name"",""CreatedDate"",""UpdatedDate"") VALUES ('Special Educational Needs and Disability','{DateTime.UtcNow}','{DateTime.UtcNow}')
            SELECT @caseTypeId = SCOPE_IDENTITY()\
            INSERT INTO HearingType (""Name"", ""CaseTypeId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('Case Management Hearing',@caseTypeId,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingType (""Name"", ""CaseTypeId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('Final Hearing',@caseTypeId,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            DECLARE @caseRoleId1 int
            INSERT INTO CaseRole (""Name"", ""Group"", ""CaseTypeId"",""CreatedDate"",""UpdatedDate"") VALUES ('Appellant', {(int) CaseRoleGroup.Appellant} , @caseTypeId,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            SELECT @caseRoleId1 = SCOPE_IDENTITY()
            DECLARE @caseRoleId2 int
            INSERT INTO CaseRole (""Name"", ""Group"", ""CaseTypeId"",""CreatedDate"",""UpdatedDate"") VALUES ('Local Authority', {(int) CaseRoleGroup.LocalAuthority}  , @caseTypeId,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            SELECT @caseRoleId2 = SCOPE_IDENTITY()
            DECLARE @caseRoleId3 int
            INSERT INTO CaseRole (""Name"", ""Group"", ""CaseTypeId"",""CreatedDate"",""UpdatedDate"") VALUES ('Observer', {(int) CaseRoleGroup.Observer}  , @caseTypeId,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            SELECT @caseRoleId3 = SCOPE_IDENTITY()
            DECLARE @caseRoleId4 int
            INSERT INTO CaseRole (""Name"", ""Group"", ""CaseTypeId"",""CreatedDate"",""UpdatedDate"") VALUES ('Panel Member', {(int)CaseRoleGroup.PanelMember}  , @caseTypeId,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            SELECT @caseRoleId4 = SCOPE_IDENTITY()
            DECLARE @caseRoleId5 int
            INSERT INTO CaseRole (""Name"", ""Group"", ""CaseTypeId"",""CreatedDate"",""UpdatedDate"") VALUES ('Judge', {(int) CaseRoleGroup.Judge}  , @caseTypeId,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            SELECT @caseRoleId5 = SCOPE_IDENTITY()
            declare @UserRoleId1 int
            declare @UserRoleId2 int
            select @UserRoleId1 = Id from UserRole where Name = 'Individual';
            select @UserRoleId2 = Id from UserRole where Name = 'Representative';
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Appellant}', @UserRoleId1, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Expert}', @UserRoleId1, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Intermediary}', @UserRoleId1, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Interpreter}', @UserRoleId1, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.LitigantInPerson}', @UserRoleId1, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.LitigationFriend}', @UserRoleId1, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.MacKenzieFriend}', @UserRoleId2, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Representative}', @UserRoleId2, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Solicitor}', @UserRoleId2, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Witness}', @UserRoleId2, @caseRoleId1,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Representative}', @UserRoleId2, @caseRoleId2,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Witness}', @UserRoleId2, @caseRoleId2,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
            INSERT INTO HearingRole (""Name"", ""UserRoleId"", ""CaseRoleId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('{Helper.HearingRoles.Observer}', @UserRoleId2, @caseRoleId3,1,'{DateTime.UtcNow}','{DateTime.UtcNow}')
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
