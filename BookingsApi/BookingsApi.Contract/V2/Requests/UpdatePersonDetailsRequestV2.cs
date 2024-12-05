namespace BookingsApi.Contract.V2.Requests;

/// <summary>
/// Request to update a person's details.
/// </summary>
public class UpdatePersonDetailsRequestV2
{
    /// <summary>
    ///     Participant first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///     Participant last name.
    /// </summary>
    public string LastName { get; set; }
        
    /// <summary>
    ///     Participant Username
    /// </summary>
    public string Username { get; set; }
}