using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class UploadWorkAllocationRequest
    {
        public string Username { get; set; }
        public List<DayWorkHours> DayWorkHours { get; set; } = new List<DayWorkHours>();
    }
}
