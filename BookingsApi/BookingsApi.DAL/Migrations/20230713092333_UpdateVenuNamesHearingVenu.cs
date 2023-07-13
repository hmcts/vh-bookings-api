using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using BookingsApi.Domain;
using System.Linq;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class UpdateVenuNamesHearingVenu : Migration
    {
        private IDictionary<string, Tuple<string, string>> venues = new Dictionary<string, Tuple<string, string>>()
        {
            { "Ayr",new Tuple<string, string>("Ayr Social Security and Child Support Tribunal", "206150") },
            { "Bath Law Courts (Civil and Family",new Tuple<string, string>("Bath Magistrates Court and Family Court", "411234") },
            { "Birkenhead County Court",new Tuple<string, string>("Birkenhead County Court and Family Court", "444097") },
            { "Birmingham Employment Tribunal",new Tuple<string, string>("Centre City Tower", "877347") },
            { "Blackburn Family Court",new Tuple<string, string>("Blackburn County Court and Family Court", "150431") },
            { "Blackpool Family Court",new Tuple<string, string>("Blackpool County Court and Family Court", "214320") },
            { "Bodmin Law Courts",new Tuple<string, string>("Bodmin County Court and Family Court ", "271813") },
            { "Bolton Crown Court",new Tuple<string, string>("Bolton Combined Court", "447533") },
            { "Brighton County Court",new Tuple<string, string>("Brighton County and Family Court", "478896") },
            { "Brighton Social Security and Child Support Tribunal",new Tuple<string, string>("Brighton Tribunal Hearing Centre", "296806") },
            { "Bristol Magistrates Court and Tribunals Hearing Centre",new Tuple<string, string>("Bristol Magistrates Court", "781155") },
            { "Carmarthen County Court and Family Court",new Tuple<string, string>("Carmarthen County Court and Tribunal Hearing Centre", "101959") },
            { "Central Family Court",new Tuple<string, string>("Central Family Court (First Avenue House)", "356855") },
            { "Chelmsford Justice Centre",new Tuple<string, string>("Chelmsford County and Family Court)", "816875") },
            { "Chesterfield Justice Centre",new Tuple<string, string>("Chesterfield Magistrates", "652852") },
            { "Crewe Magistrates Court",new Tuple<string, string>("Crewe (South Cheshire) Magistrates Court", "566296") },
            { "Derby Magistrates Court",new Tuple<string, string>("Derby Magistrates", "484482") },
            { "Dundee Tribunal Hearing Centre",new Tuple<string, string>("Dundee Tribunal Hearing Centre - Endeavour House", "367564") },
            { "Durham County Court and Family Court",new Tuple<string, string>("Durham Justice Centre ", "491107") },
            { "East Berkshire Magistrates Court",new Tuple<string, string>("East Berkshire Magistrates Court, Maidenhead", "345045") },
            { "Exeter Law Courts",new Tuple<string, string>("Exeter Combined Court", "735217") },
            { "Hull and Holderness Magistrates Court and Hearing Centre",new Tuple<string, string>("Hull Magistrates Court", "362420") },
            { "Lavender Hill Magistrates Court",new Tuple<string, string>("Lavender Hill Magistrates Court (Formerly South Western Magistrates Court)", "536548") },
            { "Leicester County Court and Family Court",new Tuple<string, string>("Leicester County Court", "223503") },
            { "Leyland Family Hearing Centre",new Tuple<string, string>("Leyland Family Court", "415903") },
            { "Luton and South Bedfordshire Magistrates Court",new Tuple<string, string>("Luton and South Bedfordshire Magistrates Court and Family Court", "252292") },
            { "Maidstone Magistrates Court",new Tuple<string, string>("Maidstone Magistrates Court and Family Court ", "782795") },
            { "Manchester Employment Tribunal",new Tuple<string, string>("Manchester Tribunal Hearing Centre - Alexandra House", "301017") },
            { "Medway Magistrates Court and Family Court",new Tuple<string, string>("Medway Magistrates Court", "771467") },
            { "Mold Justice Centre (Mold Law Courts)",new Tuple<string, string>("Mold Justice Centre", "211138") },
            { "Newton Aycliffe Magistrates Court",new Tuple<string, string>("Newton Aycliffe Magistrates Court and Family Court", "659436") },
            { "Northampton Crown, County and Family Court",new Tuple<string, string>("Northampton Crown Court, County Court and Family Court", "195489") },
            { "Oxford Magistrates Court",new Tuple<string, string>("Oxford and Southern Oxfordshire Magistrates Court", "732661") },
            { "Plymouth (St Catherine's House)",new Tuple<string, string>("Plymouth As St Catherine's House", "235590") },
            { "Preston Crown Court and Family Court (Sessions House)",new Tuple<string, string>("Preston Crown Court", "102476") },
            { "Reedley Family Hearing Centre",new Tuple<string, string>("Reedley Magistrates Court and Family Court", "739294") },
            { "Sheffield Designated Family Court",new Tuple<string, string>("Sheffield Family Hearing Centre", "778638") },
            { "Shrewsbury Crown Court",new Tuple<string, string>("Shrewsbury Justice Centre", "259170") },
            { "Skipton County Court and Family Court",new Tuple<string, string>("Skipton Magistrates and County Court", "318324") },
            { "South Tyneside County Court and Family Court",new Tuple<string, string>("South Tyneside Magistrates Court and Family Court", "563156") },
            { "Southend Crown Court",new Tuple<string, string>("Southend Combined - Crown, Mags, County and Family Court", "781139") },
            { "Stockport Magistrates Court",new Tuple<string, string>("Stockport Magistrates Court and Famiy Court", "560788") },
            { "Swansea Civil Justice Centre",new Tuple<string, string>("Swansea Civil and Family Justice Centre", "234946") },
            { "West Hampshire (Southampton) Magistrates Court",new Tuple<string, string>("West Hampshire Magistrates Court", "330480") },
            { "Wolverhampton Social Security and Child Support Tribunal",new Tuple<string, string>("Wolverhampton Ast - Norwich Union House, Wolverhampton", "788436") },
            { "Worcester Justice Centre",new Tuple<string, string>("Worcester Magistrates Court", "703200") },
            { "Worthing County Court and Family Court",new Tuple<string, string>("Worthing Magistrates and County Court", "493880") },
            { "Wrexham County and Family Court",new Tuple<string, string>("Wrexham Law Courts", "637145") },
            { "King's Lynn Crown Court",new Tuple<string, string>("KINGS LYNN CROWN COURT (& Magistrates)", "671879") },
            
        };
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < venues.Count; i++)
            {
                migrationBuilder.UpdateData(
                    table: nameof(HearingVenue),
                    keyColumn: "Name",
                    keyValue: venues.ElementAt(i).Key,
                    columns: new[] { "Name", "VenueCode" },
                    values: new object[]
                        {venues.ElementAt(i).Value.Item1, venues.ElementAt(i).Value.Item2}
                );
            }
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < venues.Count; i++)
            {
                migrationBuilder.UpdateData(
                    table: nameof(HearingVenue),
                    keyColumn: "Name",
                    keyValue: venues.ElementAt(i).Value.Item1,
                    columns: new[] { "Name", "VenueCode" },
                    values: new object[]
                        {venues.ElementAt(i).Key, null}
                );
            }
        }
    }
}
