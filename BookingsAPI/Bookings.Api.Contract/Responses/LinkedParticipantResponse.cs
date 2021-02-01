using System;
using Bookings.Domain.Enumerations;

namespace Bookings.Api.Contract.Responses
{
    public class LinkedParticipantResponse
    {
        public Guid LinkedId { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}