namespace BookingsApi.Contract.V1.Responses;

public class HearingNotificationResponse
{
    /// <summary>
    /// This is the original hearing
    /// </summary>
    public HearingDetailsResponse Hearing { get; set; }

    /// <summary>
    /// This is the total days of the multi day hearing included the first day.
    /// If TotalDays is 1 then the hearing will be treated as single day.
    /// </summary>
    public int TotalDays { get; set; }
    
    /// <summary>
    /// The first day of the multi day hearing, only applicable to multi day hearings
    /// </summary>
    public HearingDetailsResponse SourceHearing { get; set; }
}