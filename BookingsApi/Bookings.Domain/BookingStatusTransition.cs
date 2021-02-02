using Bookings.Domain.Enumerations;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.Domain
{
    public class BookingStatusTransition
    {
        private readonly List<KeyValuePair<BookingStatus, BookingStatus>> _permissibleTransitions;

        public BookingStatusTransition()
        {
            _permissibleTransitions = new List<KeyValuePair<BookingStatus, BookingStatus>>
            {
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Created),
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Cancelled),
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Created, BookingStatus.Cancelled),
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Created, BookingStatus.Failed)
            };
        }

        public bool IsValid(StatusChangedEvent statusChangedEvent)
        {
            return _permissibleTransitions.Any(x => x.Key == statusChangedEvent.CurrentStatus && x.Value == statusChangedEvent.NewStatus);
        }
    }
}
