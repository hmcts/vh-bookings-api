namespace BookingsApi.Contract.V2.Enums
{
    public enum BookingStatusV2
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
        Failed = 4,
        /// <summary>
        /// Booked without judge
        /// </summary>
        BookedWithoutJudge = 5,
        /// <summary>
        /// Confirmed without judge
        /// </summary>
        ConfirmedWithoutJudge = 6
    }
}