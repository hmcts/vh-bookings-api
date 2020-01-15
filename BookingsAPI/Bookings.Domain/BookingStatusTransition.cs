using Bookings.Domain.Enumerations;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.Domain
{
    public class BookingStatusTransition
    {
        private readonly List<KeyValuePair<BookingStatus, BookingStatus>> _permissibleTransistions;

        public BookingStatusTransition()
        {
            _permissibleTransistions = new List<KeyValuePair<BookingStatus, BookingStatus>>
            {
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Created),
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Cancelled),
                new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Created, BookingStatus.Cancelled)
            };
        }

        public bool IsValid(StatusChangedEvent statusChangedEvent)
        {
            return _permissibleTransistions.Any(x => x.Key == statusChangedEvent.CurrentStatus && x.Value == statusChangedEvent.NewStatus);
        }
    }
}
