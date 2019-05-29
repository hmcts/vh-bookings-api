using Bookings.Domain.Ddd;
using System;

namespace Bookings.Domain
{
    public class SuitabilityAnswer : Entity<long>
    {
        public SuitabilityAnswer(Guid personId, string key, string data)
        {
            PersonId = personId;
            Key = key;
            Data = data;
            CreatedDate = DateTime.UtcNow;
        }

        public Guid PersonId { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public string ExtendedData { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Person Person {get;set;}
    }
}
