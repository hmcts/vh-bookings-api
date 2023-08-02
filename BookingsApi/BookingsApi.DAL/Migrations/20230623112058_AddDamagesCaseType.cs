using System;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddDamagesCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(CaseType),
                new[] { "Id", "Name", "JurisdictionId", "ServiceId" },
                new object[,]
                {
                    { 54, "Damages", 2, "AAA7" }
                });
            
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId", "Code" },
                new object[,]
                {
                    { 300, "Disposal Hearing", 54, "AAA7-DIS" },
                    { 301, "Trial", 54, "AAA7-TRI" },
                    { 302, "Application Hearings", 54, "AAA7-APP" },
                });

            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] { "Id", "Name", "Group", "CaseTypeId" },
                values: new object[,]
                {
                    {350, "Claimant", (int) CaseRoleGroup.Claimant, 54},
                    {351, "Defendant", (int) CaseRoleGroup.Defendant, 54},
                    {352, "Judge", (int) CaseRoleGroup.Judge, 54},
                    {353, "Observer", (int) CaseRoleGroup.Observer, 54},
                    {354, "Panel Member", (int) CaseRoleGroup.PanelMember, 54},
                    {355, "Staff Member", (int) CaseRoleGroup.StaffMember, 54}
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // Claimant (350)
                    { 1139, "Barrister", 6, 350 },
                    { 1140, "Expert", 5, 350 },
                    { 1141, "Intermediary", 5, 350 },
                    { 1142, "Interpreter", 5, 350 },
                    { 1143, "Litigant in person", 5, 350 },
                    { 1144, "Litigation friend", 5, 350 },
                    { 1145, "MacKenzie friend", 5, 350 },
                    { 1146, "Representative", 6, 350 },
                    { 1147, "Solicitor", 6, 350 },
                    { 1148, "Witness", 5, 350 },
                    
                    // Defendant (351)
                    { 1149, "Barrister", 6, 351 },
                    { 1150, "Expert", 5, 351 },
                    { 1151, "Intermediary", 5, 351 },
                    { 1152, "Interpreter", 5, 351 },
                    { 1153, "Litigant in person", 5, 351 },
                    { 1154, "Litigation friend", 5, 351 },
                    { 1155, "MacKenzie friend", 5, 351 },
                    { 1156, "Representative", 6, 351 },
                    { 1157, "Solicitor", 6, 351 },
                    { 1158, "Witness", 5, 351 },
                    
                    // Judge (352)
                    { 1159, "Judge", 4, 352 },
                    
                    // Observer (353)
                    { 1160, "Observer", 5, 353 },
                    
                    // Panel Member (354)
                    { 1161, "Panel Member", 7, 354 },
                    
                    // Staff Member (355)
                    { 1162, "Staff Member", 8, 355 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingRoleId = 1139; hearingRoleId <= 1162; hearingRoleId++)
            {
                migrationBuilder.DeleteData(
                    table: nameof(HearingRole),
                    keyColumn: "Id",
                    keyValue: hearingRoleId);
            }
            
            for (int caseRoleId = 350; caseRoleId <= 355; caseRoleId++)
            {
                migrationBuilder.DeleteData(
                    table: nameof(CaseRole),
                    keyColumn: "Id",
                    keyValue: caseRoleId);
            }
            
            for (int hearingTypeId = 300; hearingTypeId <= 302; hearingTypeId++)
            {
                migrationBuilder.DeleteData(
                    table: nameof(HearingType),
                    keyColumn: "Id",
                    keyValue: hearingTypeId);
            }
            
            migrationBuilder.DeleteData(
                table: nameof(CaseType),
                keyColumn: "Id",
                keyValue: 54);
        }
    }
}
