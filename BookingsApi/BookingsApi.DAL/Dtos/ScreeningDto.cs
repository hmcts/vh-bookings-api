namespace BookingsApi.DAL.Dtos;

public class ScreeningDto
{
    /// <summary>
    /// Screening type (all or specific)
    /// </summary>
    public ScreeningType ScreeningType { get; set; }

    /// <summary>
    /// The external reference id of entities to protect from
    /// </summary>
    public List<string> ProtectedFrom { get; set; } = [];
}