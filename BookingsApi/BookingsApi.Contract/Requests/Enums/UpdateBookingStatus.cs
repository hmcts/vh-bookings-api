namespace BookingsApi.Contract.Requests.Enums
{
    /// <summary>
    ///     Booking status to update
    /// </summary>
    public enum UpdateBookingStatus
    {
        Booked = 1,
        Created = 2,
        Cancelled = 3,
        Failed = 4
    }
}
