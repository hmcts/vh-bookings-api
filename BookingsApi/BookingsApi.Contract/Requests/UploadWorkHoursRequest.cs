using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class UploadWorkHoursRequest
    {
        public string Username { get; set; }
        public List<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
    }
}
