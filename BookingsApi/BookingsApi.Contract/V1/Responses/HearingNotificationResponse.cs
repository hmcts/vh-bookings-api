namespace BookingsApi.Contract.V1.Responses;

public class HearingNotificationResponse
{
    /// <summary>
    /// This is the original hearing
    /// </summary>
    public HearingDetailsResponse Hearing { get; set; }

    public int TotalDays { get; set; }
}