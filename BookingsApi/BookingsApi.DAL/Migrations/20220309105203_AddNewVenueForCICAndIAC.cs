using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingsApi.DAL.Migrations
{
    public partial class AddNewVenueForCICAndIAC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: nameof(HearingVenue),
              columns: new[] { "Id", "Name" },
              values: new object[,]
              {
                    { 33, "Ashford Tribunal Hearing Centre" },
                    { 34, "Bradford Tribunal Hearing Centre" },
                    { 35, "Brighton Social Security and Child Support Tribunal" },
                    { 36, "Cardiff Civil and Family Justice Centre" },
                    { 37, "East London Tribunal Hearing Centre" },
                    { 38, "King's Lynn Magistrates Court and Family Court" },
                    { 39, "Liverpool Civil and Family Court" },
                    { 40, "Manchester Tribunal Hearing Centre" },
                    { 41, "North Shields County Court and Family Court" },
                    { 42, "Nottingham Justice Centre" },
                    { 43, "Plymouth (St Catherine's House)" },
                    { 44, "Port Talbot Justice Centre" },
                    { 45, "Sheffield Magistrates Court" },
                    { 46, "Wolverhampton Social Security and Child Support Tribunal" },
                    { 47, "Laganside Courts" },
                    { 48, "Yarl's Wood Immigration and Asylum Hearing Centre" },
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int hearingVenueId = 33; hearingVenueId <= 48; hearingVenueId++)
            {
                migrationBuilder.DeleteData(nameof(HearingVenue), "Id", hearingVenueId);
            }
        }
    }
}
