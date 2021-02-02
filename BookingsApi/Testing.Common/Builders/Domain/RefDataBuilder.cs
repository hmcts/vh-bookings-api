using System.Collections.Generic;
using Bookings.Domain;

namespace Testing.Common.Builders.Domain
{
    public class RefDataBuilder
    {
        public List<HearingVenue> HearingVenues { get; set; }


        public RefDataBuilder()
        {
            InitHearingVenues();
        }
        
        private void InitHearingVenues()
        {
           HearingVenues = new List<HearingVenue>()
           {
               new HearingVenue(1, "Birmingham Civil and Family Justice Centre"),
               new HearingVenue(2, "Manchester Civil and Family Justice Centre"),
               new HearingVenue(3, "Taylor House Tribunal Hearing Centre"),
           };
        }
    }
}