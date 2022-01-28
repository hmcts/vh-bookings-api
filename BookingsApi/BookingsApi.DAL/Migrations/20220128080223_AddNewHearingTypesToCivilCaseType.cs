using Microsoft.EntityFrameworkCore.Migrations;
using BookingsApi.Domain.RefData;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewHearingTypesToCivilCaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                nameof(HearingType),
                new[] { "Id", "Name", "CaseTypeId" },
                new object[,]
                {
                    { 186, "Civil Enforcement", 8 },
                    { 187, "Mortgage and Landlord Possession Claims", 8 },
                    { 188, "Non-money Claims", 8 },
                    { 189, "Return of Goods Claims", 8 },
                    { 190, "Specified Money Claims", 8 },
                    { 191, "Damages", 8 },
                    { 192, "Antisocial Behaviour", 8 },
                    { 193, "Appeal", 8 },
                    { 194, "Approval of a Prohibited Party", 8 },
                    { 195, "Disposal", 8 },
                    { 196, "Gas Injunction", 8 },
                    { 197, "Interim third party order", 8 },
                    { 198, "Interlocutory judgement", 8 },
                    { 199, "Judgement", 8 },
                    { 200, "Other", 8 },
                    { 201, "Pre-action disclosure", 8 },
                    { 202, "Pre-trial review", 8 },
                    { 203, "Protected party settlement", 8 },
                    { 204, "Redetermination", 8 },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingTypeId = 186; hearingTypeId <= 204; hearingTypeId++)
            {
                migrationBuilder.DeleteData(nameof(HearingType), "Id", hearingTypeId);
            }
        }
    }
}
