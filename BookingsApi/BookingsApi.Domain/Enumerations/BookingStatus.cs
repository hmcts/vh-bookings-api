namespace BookingsApi.Domain.Enumerations
{
    public enum BookingStatus
    {
        Booked = 1,
        Created = 2,
        Cancelled = 3,
        Failed = 4,
        BookedWithoutJudge = 5,
        ConfirmedWithoutJudge = 6
    }
}