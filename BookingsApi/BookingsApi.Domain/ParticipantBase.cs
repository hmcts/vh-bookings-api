using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public abstract class ParticipantBase : TrackableEntity<Guid>
    {
        public string DisplayName { get; set; }
    }
}
