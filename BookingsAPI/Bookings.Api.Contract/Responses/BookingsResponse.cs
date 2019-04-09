using System.Collections.Generic;

namespace Bookings.Api.Contract.Responses
{
    public class BookingsResponse : PagedCursorBasedResponse
    {
        /// <summary>
        /// Gets or sets list of bookings hearings.
        /// </summary>
        public List<BookingsByDateResponse> Hearings { get; set; }
    }
}