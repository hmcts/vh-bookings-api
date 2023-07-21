
namespace BookingsApi.Contract.V1.Requests
{
    public class CancelBookingRequest
    {
        /// <summary>
        ///  The user requesting to update the status
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// The reason for cancelling the video hearing
        /// </summary>
        public string CancelReason { get; set; }
    }


}
