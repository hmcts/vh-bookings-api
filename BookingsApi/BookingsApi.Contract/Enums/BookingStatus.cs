namespace BookingsApi.Contract.Enums
{
    public enum BookingStatus
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