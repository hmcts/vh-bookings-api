using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class UpdateHearingAllocationToCsoRequest
    {
        /// <summary>
        ///     List of removed participant Ids
        /// </summary>
        public List<Guid> Hearings { get; set; }

        /// <summary>
        ///     List of linked participants
        /// </summary>
        public Guid CsoId { get; set; }
    }
}