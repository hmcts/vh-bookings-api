using Bookings.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Domain
{
    public class StatusChangedEvent
    {
        public StatusChangedEvent(BookingStatus currentStatus, BookingStatus newStatus)
        {
            CurrentStatus = currentStatus;
            NewStatus = newStatus;
        }

        public BookingStatus CurrentStatus { get; }
        public BookingStatus NewStatus { get; }

    }
}
