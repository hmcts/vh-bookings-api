using System.Collections.Generic;

namespace BookingsApi.Contract.Responses;

public class VhoSearchResponse
{
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string Lastname { get; set; }
    public string ContactEmail { get; set; }
    public string Telephone { get; set; }
    public string UserRole { get; set; }
    public List<VhoWorkHoursResponse> VhoWorkHours { get; set; }
}