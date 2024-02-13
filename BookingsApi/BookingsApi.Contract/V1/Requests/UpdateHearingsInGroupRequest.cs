using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateHearingsInGroupRequest
    {
        /// <summary>
        /// List of hearings in the group to update
        /// </summary>
        public List<HearingRequest> Hearings { get; set; } = new();
    }
}
