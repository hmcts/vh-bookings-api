using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class BookingsByDateResponse
    {
        /// <summary>
        /// The hearings grouped by date without time.
        /// </summary>
        public DateTime ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets list of bookings hearings.
        /// </summary>
        public List<BookingsHearingResponse> Hearings { get; set; }
    }
}