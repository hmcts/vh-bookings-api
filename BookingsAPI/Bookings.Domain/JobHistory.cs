using Bookings.Domain.Ddd;
using System;

namespace Bookings.Domain
{
    public class JobHistory : Entity<Guid>
    {
        protected JobHistory()
        {
            Id = Guid.NewGuid();
            LastRunDate = DateTime.UtcNow;
        }
        public DateTime? LastRunDate { get; protected set; }
    }
}
