using System;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Responses
{
    public class LinkedParticipantResponseV2
    {
        /// <summary>
        ///     The linked participant in the response
        /// </summary>
        public Guid LinkedId { get; set; }
        
        /// <summary>
        ///     The type of the linked participant
        /// </summary>
        public LinkedParticipantTypeV2 TypeV2 { get; set; }
    }
}