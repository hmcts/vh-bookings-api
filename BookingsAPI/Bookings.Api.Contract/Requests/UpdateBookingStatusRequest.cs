using Bookings.Api.Contract.Requests.Enums;

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
        public UpdateBookingStatus Status { get; set; }
    }


}
