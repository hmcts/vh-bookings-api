using System.Collections.Generic;
using Bookings.Domain;
using Bookings.Domain.RefData;

namespace Testing.Common.Builders.Domain
{
    public class RefDataBuilder
    {
        public List<HearingVenue> HearingVenues { get; set; }
        public List<CaseType> CaseTypes { get; set; }


        public RefDataBuilder()
        {
            CaseTypes = new List<CaseType>();
            InitHearingVenues();
        }
        
        private void InitHearingVenues()
        {
           HearingVenues = new List<HearingVenue>()
           {
               new HearingVenue(1, "Birmingham Civil and Family Justice Centre"),
               new HearingVenue(2, "Manchester Civil and Family Justice Centre")
           };
        }
    }
}