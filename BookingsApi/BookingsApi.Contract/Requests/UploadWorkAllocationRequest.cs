using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class UploadWorkAllocationRequest
    {
        public string Username { get; set; }
        public List<WorkHours> WorkHours { get; set; } = new List<WorkHours>();
    }
}
