using System.Collections.Generic;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdateHearingsInGroupRequestV2
    {
        /// <summary>
        /// List of hearings in the group to update
        /// </summary>
        public List<HearingRequestV2> Hearings { get; set; } = new();
    }
}
