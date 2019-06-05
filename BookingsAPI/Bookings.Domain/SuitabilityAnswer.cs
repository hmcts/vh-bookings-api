using Bookings.Domain.Ddd;
using Bookings.Domain.Participants;
using System;

namespace Bookings.Domain
{
    public class SuitabilityAnswer : Entity<long>
    {
        public SuitabilityAnswer(string key, string data, string extendedData)
        {
            Key = key;
            Data = data;
            ExtendedData = extendedData;
            UpdatedDate = DateTime.UtcNow;
        }

        public Guid ParticipantId { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public string ExtendedData { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Participant Participant {get;set;}
    }
}
