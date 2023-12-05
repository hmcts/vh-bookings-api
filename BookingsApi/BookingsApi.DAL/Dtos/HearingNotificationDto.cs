namespace BookingsApi.DAL.Dtos;

/// <summary>
/// Extended class of the original dto with an additional property of TotalDays
/// </summary>
/// <param name="Hearing"></param>
/// <param name="TotalDays"></param>
/// <param name="SourceHearing"></param>
public record HearingNotificationDto(VideoHearing Hearing, int TotalDays, VideoHearing SourceHearing = null);