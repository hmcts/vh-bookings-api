using System;

namespace BookingsApi.Contract.V1.Requests;

/// <summary>
/// Add a justice user
/// </summary>
public class RestoreJusticeUserRequest
{
    /// <summary>
    /// The user's ID
    /// </summary>
    public Guid Id { get;set; }
    
    /// <summary>
    /// The user's username
    /// </summary>
    public string Username { get;set; }
}