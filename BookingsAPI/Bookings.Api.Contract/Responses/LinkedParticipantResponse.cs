using System;
using Bookings.Domain.Enumerations;

namespace Bookings.Api.Contract.Responses
{
    public class LinkedParticipantResponse
    {
        /// <summary>
        ///     The linked participant in the response
        /// </summary>
        public Guid LinkedId { get; set; }
        
        /// <summary>
        ///     The type of the linked participant
        /// </summary>
        public LinkedParticipantType Type { get; set; }
    }
}