using System;

namespace BookingsApi.Domain
{
    public abstract class ParticipantBase : TrackableEntity<Guid>
    {
        protected ParticipantBase()
        {
            Id = Guid.NewGuid();
        }
        
        public string DisplayName { get; set; }
    }
}
