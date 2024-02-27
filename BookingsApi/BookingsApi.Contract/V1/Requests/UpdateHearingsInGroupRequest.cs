using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateHearingsInGroupRequest
    {
        /// <summary>
        /// Name of the user updating the hearings
        /// </summary>
        public string UpdatedBy { get; set; }
        
        /// <summary>
        /// List of hearings in the group to update
        /// </summary>
        public List<HearingRequest> Hearings { get; set; } = new();
    }
}
