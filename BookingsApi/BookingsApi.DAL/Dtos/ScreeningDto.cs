namespace BookingsApi.DAL.Dtos;

public class ScreeningDto
{
    /// <summary>
    /// Screening type (all or specific)
    /// </summary>
    public ScreeningType ScreeningType { get; set; }
    
    /// <summary>
    /// The contact emails of participants to screen from
    /// </summary>
    public List<string> ProtectFromParticipants { get; set; }
    
    /// <summary>
    /// The display names of endpoints to screen from
    /// </summary>
    public List<string> ProtectFromEndpoints { get; set; }
}