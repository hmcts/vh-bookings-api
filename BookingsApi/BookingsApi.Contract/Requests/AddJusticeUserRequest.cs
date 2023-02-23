using BookingsApi.Contract.Requests.Enums;

namespace BookingsApi.Contract.Requests;

/// <summary>
/// Add a justice user
/// </summary>
public class AddJusticeUserRequest
{
    public AddJusticeUserRequest()
    {
        // default to VHO to avoid accidentally assigned the role TeamLead
        Role = JusticeUserRole.Vho;
    }
    
    /// <summary>
    /// The user's first name
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// The user's last name
    /// </summary>
    public string LastName { get;set; }
    
    /// <summary>
    /// The user's username
    /// </summary>
    public string Username { get;set; }
    
    /// <summary>
    /// The user's contact email
    /// </summary>
    public string ContactEmail { get; set;}
    
    /// <summary>
    /// The user's telephone
    /// </summary>
    public string Telephone { get; set; }
    
    /// <summary>
    /// The username of the person who requested the creation of this entry
    /// </summary>
    public string CreatedBy { get; set;}
    
    /// <summary>
    /// The user's role. This can be a VHO or a Team Lead.
    /// </summary>
    public JusticeUserRole Role { get; set;}
}