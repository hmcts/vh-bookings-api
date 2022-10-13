using System.Collections.Generic;

namespace BookingsApi.Contract.Responses;

public class VhoSearchResponse
{
    public string Username { get; set; }
    public List<VhoWorkHoursResponse> VhoWorkHours { get; set; }
}