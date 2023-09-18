using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain
{
    public class BookingStatusTransition
    {
        private readonly List<KeyValuePair<BookingStatus, BookingStatus>> _permissibleTransitions = new()
        {
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Created),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.BookedWithoutJudge),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.ConfirmedWithoutJudge),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Cancelled),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Created, BookingStatus.Cancelled),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Created, BookingStatus.ConfirmedWithoutJudge),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Booked, BookingStatus.Failed),
            new KeyValuePair<BookingStatus, BookingStatus>(BookingStatus.Failed, BookingStatus.Created)
        };

        public bool IsValid(StatusChangedEvent statusChangedEvent)
        {
            return _permissibleTransitions.Exists(x => x.Key == statusChangedEvent.CurrentStatus && x.Value == statusChangedEvent.NewStatus);
        }
    }
}
