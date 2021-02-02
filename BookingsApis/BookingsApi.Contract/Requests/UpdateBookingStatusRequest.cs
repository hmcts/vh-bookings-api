using BookingsApi.Contract.Requests.Enums;

namespace BookingsApi.Contract.Requests
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

        /// <summary>
        /// The reason for cancelling the video hearing
        /// </summary>
        public string CancelReason { get; set; }
    }


}
