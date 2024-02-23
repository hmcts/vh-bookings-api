using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class CancelHearingsInGroupRequest
    {
        /// <summary>
        /// List of hearing ids in the group to cancel
        /// </summary>
        public List<Guid> HearingIds { get; set; }
        
        /// <summary>
        ///  The user requesting to update the status
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// The reason for cancelling the video hearing
        /// </summary>
        public string CancelReason { get; set; }
    }
}
