using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateHearingsInGroupRequestV2
    {
        public List<HearingRequestV2> Hearings { get; set; } = new();
    }
    
    // TODO move the below into separate files

}
