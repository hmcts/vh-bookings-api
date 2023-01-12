namespace BookingsApi.Contract.Requests.Enums
{
    /// <summary>
    ///     Booking status to update
    /// </summary>
    public enum UpdateBookingStatus
    {
        /// <summary>
        /// Hearing booked
        /// </summary>
        Booked = 1,
        /// <summary>
        /// Conference created
        /// </summary>
        Created = 2,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 3,
        /// <summary>
        /// Failed
        /// </summary>
        Failed = 4
    }
}
