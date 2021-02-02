﻿using Bookings.Domain.Enumerations;

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
