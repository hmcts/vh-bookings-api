using Bookings.Domain.Ddd;
using Bookings.Domain.Participants;
using System;

namespace Bookings.Domain
{
    public class SuitabilityAnswer : Entity<long>
    {
        public SuitabilityAnswer(Guid participantId, string key, string data)
        {
            ParticipantId = participantId;
            Key = key;
            Data = data;
            CreatedDate = DateTime.UtcNow;
        }

        public Guid ParticipantId { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public string ExtendedData { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Participant Participant {get;set;}
    }
}
