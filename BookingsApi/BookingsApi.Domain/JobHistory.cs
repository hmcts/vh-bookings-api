using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
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
