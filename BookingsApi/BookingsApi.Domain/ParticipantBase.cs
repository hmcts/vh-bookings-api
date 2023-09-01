using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public abstract class ParticipantBase : Entity<Guid>
    {
        public string DisplayName { get; set; }
    }
}
