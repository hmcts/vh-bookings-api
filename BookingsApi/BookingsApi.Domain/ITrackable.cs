using System;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain
{
    public interface ITrackable
    {
        DateTime? CreatedDate { get; set; }
        DateTime? UpdatedDate { get; set; }
    }

    public class TrackableEntity<TKey> : Entity<TKey>, ITrackable
    {
        private readonly DateTime _currentUTC = DateTime.UtcNow;
        protected TrackableEntity()
        {
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
        }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
