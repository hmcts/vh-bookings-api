using System;
using System.Collections.Generic;
using BookingsApi.Contract.Requests.Enums;

namespace BookingsApi.Contract.Requests;

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
}