using System;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class AddIsScottishAndIsWorkAllocationEnabledColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsScottish",
                table: "HearingVenue",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWorkAllocationEnabled",
                table: "HearingVenue",
                type: "bit",
                nullable: false,
                defaultValue: true);


            migrationBuilder.UpdateData(table: nameof(HearingVenue),
                keyColumn: nameof(HearingVenue.Id),
                keyValues: new object[]
                {
                    23, 337, 338, 15, 16, 17, 18, 19, 21, 49, 50, 51, 52, 30, 31
                },
                column: nameof(HearingVenue.IsWorkAllocationEnabled),
                values: new object[]
                {
                    false, false, false, false, false, false, false, false, false, false, false, false, false, false,
                    false
                });

            migrationBuilder.UpdateData(table: nameof(HearingVenue),
                keyColumn: nameof(HearingVenue.Id),
                keyValues: new object[]
                {
                    15, 16, 17, 18, 19, 21, 49, 50, 51, 52, 30, 31
                },
                column: nameof(HearingVenue.IsScottish),
                values: new object[]
                    {true, true, true, true, true, true, true, true, true, true, true, true});


            // migrationBuilder.UpdateData(
            //         table: nameof(HearingVenue), 
            //         keyColumn:nameof(HearingVenue.Id), 
            //         keyValues:[23, 337, 338],
            //         column: nameof(HearingVenue.IsWorkAllocationEnabled),
            //             values: new object[] { false, false, false });



            // non work allocation
            //{ 338, "Teesside Combined Court Centre" },
            // { 337, "Teesside Magistrates Court" },
            // {23, "Middlesbrough County Court"}

            // non work allocation and is scottish
            // new  {15, "Aberdeen Tribunal Hearing Centre"},
            //      {16, "Dundee Tribunal Hearing Centre"},
            //      {17, "Edinburgh Employment Tribunal"},
            //      {18, "Glasgow Tribunals Centre"},
            //      {19, "Inverness Employment Tribunal"},
            //      {21, "Ayr"},
            //      {49, "Edinburgh Employment Appeal Tribunal" },
            //      {50, "Inverness Justice Centre" },
            //      {51, "Edinburgh Social Security and Child Support Tribunal" },
            //      {52, "Edinburgh Upper Tribunal (Administrative Appeals Chamber)" },
            //      {30, "Hamilton Brandon Gate"},
            //      {31, "Stirling Wallace House"}

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScottish",
                table: "HearingVenue");

            migrationBuilder.DropColumn(
                name: "IsWorkAllocationEnabled",
                table: "HearingVenue");
        }
    }
}
