using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateHearingsInGroupRequest
    {
        public List<HearingRequest> Hearings { get; set; } = new();
    }
}
