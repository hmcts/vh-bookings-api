using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdateHearingAllocationToCsoRequest
    {
        /// <summary>
        ///     List of hearing Ids
        /// </summary>
        public List<Guid> Hearings { get; set; }

        /// <summary>
        ///     Cso id
        /// </summary>
        public Guid CsoId { get; set; }
    }
}