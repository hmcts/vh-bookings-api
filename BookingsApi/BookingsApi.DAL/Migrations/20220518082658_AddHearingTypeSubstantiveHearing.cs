using System;
using BookingsApi.Contract.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddHearingTypeSubstantiveHearing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
var sqlScript = $@"
            declare @caseTypeId int
            -- get CaseTypeId
            select @caseTypeId = Id from CaseType
            where Name = 'Upper Tribunal Tax';
            INSERT INTO HearingType (""Name"", ""CaseTypeId"", ""Live"",""CreatedDate"",""UpdatedDate"") VALUES ('Substantive Hearing',@caseTypeId,1,'{DateTime.UtcNow}','{DateTime.UtcNow}');";
            
            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"
                declare @caseTypeId int
                -- get CaseTypeId
                select @caseTypeId = Id from CaseType
                where Name = 'Upper Tribunal Tax'

                -- delete HearingType 
                delete from HearingType where CaseTypeId = @caseTypeId and Name = 'Substantive Hearing'
                ";
            
            migrationBuilder.Sql(sqlScript);
        }
    }
}
