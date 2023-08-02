using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Requests
{
    public class UploadWorkHoursRequest
    {
        public string Username { get; set; }
        public List<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
    }
}
