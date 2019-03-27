using System;
using System.Collections.Generic;
using System.Text;

namespace Bookings.Api.Contract.Requests
{
    public class UpdateBookingStatusRequest
    {
        /// <summary>
        ///  The user requesting to update the status
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        ///     New status of the hearing
        /// </summary>
        public string Status { get; set; }
    }
}
