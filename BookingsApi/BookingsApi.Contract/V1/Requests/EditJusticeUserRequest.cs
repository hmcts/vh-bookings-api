using System;
using System.Collections.Generic;
using BookingsApi.Contract.V1.Requests.Enums;

namespace BookingsApi.Contract.V1.Requests;

/// <summary>
/// Add a justice user
/// </summary>
public class EditJusticeUserRequest
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid Id { get;set; }
    
    /// <summary>
    /// The user's username
    /// </summary>
    public string Username { get;set; }

    /// <summary>
    /// The user's role. This can be a VHO or a Team Lead.
    /// </summary>
    public List<JusticeUserRole> Roles { get; set;}
    
    /// <summary>
    /// The Users first name
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// The Users last name
    /// </summary>
    public string LastName { get; set; }
    
    /// <summary>
    /// The Users Contact Number
    /// </summary>
    public string ContactTelephone { get; set; }
}