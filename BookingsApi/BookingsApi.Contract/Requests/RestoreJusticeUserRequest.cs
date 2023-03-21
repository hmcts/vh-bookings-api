using System;
using BookingsApi.Contract.Requests.Enums;

namespace BookingsApi.Contract.Requests;

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