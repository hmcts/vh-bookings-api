using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingsApi.DAL.Migrations
{
    public partial class Update_Venues_With_EPIMS_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: nameof(HearingVenue),
            columns: new[] { "Id", "EpimsCode", "Name" },
            values: new object[,]
            {
                { 385, "364992", "Aldgate Tower (3rd Floor)" },
                { 386, "788490", "Alton Magistrates Court" }, 
                { 387, "999977", "Anglesey" },
                { 388, "366530", "Anglesey Shirehall" },
                { 389, "324339", "Arnhem House (Leicester Offices) Floor 1, 2, 5" },
                { 390, "648617", "Aylesbury Walton Street" },
                { 391, "816198", "Barkingside Magistrates Court (1st Floor Offices)" },
                { 392, "999999", "Benbecula" },
                { 393, "999974", "Berwick" },
                { 394, "430480", "Birmingham Cown Court Annexe (4 Newton Street)" },
                { 395, "589324", "Birmingham Masshouse Lane" },
                { 396, "815833", "Birmingham CTSC (54 Hagley Road)" },
                { 397, "107017", "Blackburn Social Security and Child Support Tribunal" },
                { 398, "196222", "Blackpool Social Security and Child Support Tribunal" },
                { 399, "632407", "Southampton Western Range/Barrack Block" },
                { 400, "443089", "Burnley Social Security and Child Support Tribunal" },
                { 401, "286116", "Camberwell Green Magistrates Court" }, 
                { 402, "999998", "Campbeltown" },
                { 403, "643902", "Central Payments Office (Middleton Stoney)" },
                { 404, "525995", "Chelmsford Offices (Osprey House)" },
                { 405, "999979", "Chichester" },
                { 406, "111544", "Chichester Combined Court Centre" },
                { 407, "228416", "Chichester Magistrates Court" },
                { 408, "525883", "Chorley Magistrates Court" },
                { 409, "999980", "Eastbourne" },
                { 410, "745385", "Exeter Magistrates Court - North and East Devon" },
                { 411, "815811", "Exeter Tribunal Hearing Centre" },
                { 412, "401452", "Fleetwood Magistrates Court" },
                { 413, "406867", "Fox Court SSCS (Holborn Courts and Tribunals)" },
                { 414, "999993", "Galashiels" },
                { 415, "366559", "Atlantic Quay Glasgow" },
                { 416, "999992", "Greenock" },
                { 417, "723075", "Hamilton Social Security and Child Support Tribunal" },
                { 418, "631056", "Harlow Social Security and Child Support Tribunal" },
                { 419, "416799", "Hartlepool Magistrates Court and Family Hearing Court" }, 
                { 420, "228883", "Hereford Magistrates Court" },
                { 421, "526052", "Hertford Offices (County Hall)" },
                { 422, "372249", "Hull Tribunal Hearing Centre" },
                { 423, "107378", "Inverness Social Security and Child Support Tribunal" },
                { 424, "571766", "Isles of Scilly Magistrates' Court" },
                { 425, "999982", "Kendal Town Hall" },
                { 426, "833753", "Kenfig Storage Unit" },
                { 427, "999991", "Kilmarnock" },
                { 428, "999990", "Kirkcaldy" },
                { 429, "999989", "Kirkwall" },
                { 430, "999975", "Knighton" },
                { 431, "353615", "Lancaster Magistrates Court" }, 
                { 432, "235806", "Leeds Civil Hearing Centre" },
                { 433, "999988", "Lerwick" },
                { 434, "366774", "Level 4 Metro" },
                { 435, "855737", "Lewes Castle Ditch Road" },
                { 436, "999986", "Lewis" },
                { 437, "366847", "Lewis Building" },
                { 438, "290690", "Maidstone Offices (Gail House)" },
                { 439, "517452", "Morris House (Loughborough Storage)" },
                { 440, "498443", "Newcastle Upon Tyne Combined Court Centre" }, 
                { 441, "462798", "Newcastle Upon Tyne Magistrates Court (Anderson House)" },
                { 442, "323394", "Newcastle Upon Tyne Magistrates Court" },
                { 443, "227101", "Newport Tribunal Centre - Columbus House" },
                { 444, "365003", "North Somerset Magistrates (Queensway House)" },
                { 445, "766969", "Northampton Offices (Regents Pavilion)" },
                { 446, "420219", "Northampton Bulk Issue Centre" },
                { 447, "999987", "Oban" },
                { 448, "558490", "Rochdale Social Security and Child Support Tribunal" },
                { 449, "339868", "Rugeley Offices (Former Magistrates Court)" },
                { 450, "491043", "Runcorn Magistrates Court" },
                { 451, "366571", "Runcorn Rutland House" },
                { 452, "495548", "Scunthorpe Magistrates' and County Court" },
                { 453, "697903", "Stevenage Offices (Bayley House)" },
                { 454, "283922", "Stoke on Trent Tribunal Hearing Centre" }, 
                { 455, "999984", "Stranraer" },
                { 456, "514973", "Stratford Magistrates Court" }, 
                { 457, "75217", "Sunderland County Court" },
                { 458, "509822", "Torquay Magistrates Court" },
                { 459, "999981", "Tunbridge Wells" },
                { 460, "379904", "Twyver House" },
                { 461, "409795", "Warrington Combined Court" }, 
                { 462, "228231", "Nuneaton Magistrates Court" },
                { 463, "736719", "Leamington Spa Magistrates' Court" }, 
                { 464, "227860", "Watford County Court Royalty House" }
            });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int Id = 385; Id <= 464; Id++)
            {
                migrationBuilder.DeleteData(
                    table: nameof(HearingVenue),
                    keyColumn: "Id",
                    keyValue: Id);
            }
        }
    }
}
