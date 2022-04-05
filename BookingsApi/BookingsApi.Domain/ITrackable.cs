using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public interface ITrackable
    {
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
    }

    public class TrackableEntity<TKey> : Entity<TKey>, ITrackable
    {
        private DateTime _createdDate = DateTime.UtcNow;
        private DateTime _updatedDate = DateTime.UtcNow;

        public DateTime CreatedDate { get => _createdDate; set => _createdDate = DateTime.UtcNow; }
        public DateTime UpdatedDate { get => _updatedDate; set => _updatedDate = DateTime.UtcNow; }
    }
}
