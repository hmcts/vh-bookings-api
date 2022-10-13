using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class UploadNonWorkingHoursRequest
    {
        public string Username { get; set; }
        public List<NonWorkingHours> NonWorkingHours { get; set; } = new List<NonWorkingHours>();
    }
}
