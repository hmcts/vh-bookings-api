using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddDivorceHearingTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(CaseType),
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 34, "Divorce" },
                });

            migrationBuilder.InsertData(
                table: nameof(HearingType),
                columns: new[] { "Id", "Name", "CaseTypeId" },
                values: new object[,]
                {
                    { 154, "Case Management Hearing", 34 },
                    { 155, "Directions Hearing", 34 },
                    { 156, "Final Hearing", 34 },
                    { 157, "Financial Dispute Resolution", 34 },
                    { 158, "First Application", 34 },
                    { 159, "First Directions Appointment", 34 },
                    { 160, "Hearing", 34 },
                    { 161, "Interim hearing", 34 },
                });

            migrationBuilder.InsertData(
                table: nameof(CaseRole),
                columns: new[] {"Id", "Name", "Group", "CaseTypeId"},
                values: new object[,]
                {
                    {182, "Appellant", (int) CaseRoleGroup.Appellant, 34},
                    {183, "Applicant", (int) CaseRoleGroup.Applicant, 34},
                    {184, "Respondent", (int) CaseRoleGroup.Respondent, 34},
                    {185, "Observer", (int) CaseRoleGroup.Observer, 34},
                });

            migrationBuilder.InsertData(
                nameof(HearingRole),
                new[] { "Id", "Name", "UserRoleId", "CaseRoleId", },
                new object[,]
                {
                    // appellant (182)
                    { 573, "Barrister", 6, 182 },
                    { 574, "Expert", 5, 182 },
                    { 575, "Intermediary", 5, 182 },
                    { 576, "Interpreter", 5, 182 },
                    { 577, "Litigant in person", 5, 182 },
                    { 578, "Litigation friend", 5, 182 },
                    { 579, "MacKenzie friend", 5, 182 },
                    { 580, "Representative", 6, 182 },
                    { 581, "Solicitor", 6, 182 },
                    { 582, "Witness", 5, 182 },
                    // applicant (183)
                    { 583, "Barrister", 6, 183 },
                    { 584, "Expert", 5, 183 },
                    { 585, "Intermediary", 5, 183 },
                    { 586, "Interpreter", 5, 183 },
                    { 587, "Litigant in person", 5, 183 },
                    { 588, "Litigation friend", 5, 183 },
                    { 589, "MacKenzie friend", 5, 183 },
                    { 590, "Representative", 6, 183 },
                    { 591, "Solicitor", 6, 183 },
                    { 592, "Witness", 5, 183 },
                    // respondent (184)
                    { 593, "Barrister", 6, 184 },
                    { 594, "Expert", 5, 184 },
                    { 595, "Intermediary", 5, 184 },
                    { 596, "Interpreter", 5, 184 },
                    { 597, "Litigant in person", 5, 184 },
                    { 598, "Litigation friend", 5, 184 },
                    { 599, "MacKenzie friend", 5, 184 },
                    { 600, "Representative", 6, 184 },
                    { 601, "Solicitor", 6, 184 },
                    { 602, "Witness", 5, 184 },
                    // observer (185)
                    { 603, "Observer", 5, 185 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("CaseType", "Id", 34);
            
            for (var i = 154; i < 162; i++)
            {
                migrationBuilder.DeleteData("HearingType", "Id", i);
            }
            
            for (var i = 182; i < 186; i++)
            {
                migrationBuilder.DeleteData("CaseRole", "Id", i);
            }

            for (var i = 573; i < 604; i++)
            {
                migrationBuilder.DeleteData("HearingRole", "Id", i);
            }
        }
    }
}
