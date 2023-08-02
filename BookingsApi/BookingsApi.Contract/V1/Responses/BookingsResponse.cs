using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Responses
{
    public class BookingsResponse : PagedCursorBasedResponse
    {
        /// <summary>
        /// Gets or sets list of bookings hearings.
        /// </summary>
        public List<BookingsByDateResponse> Hearings { get; set; }
    }
}