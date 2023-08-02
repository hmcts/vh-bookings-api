using System;

namespace BookingsApi.Contract.V1.Requests
{
    public class UploadNonWorkingHoursRequest
    {
        public string Username { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }

        public UploadNonWorkingHoursRequest(string username, DateTime startTime, DateTime endTime)
        {
            Username = username;
            EndTime = endTime;
            StartTime = startTime;
        }
    }
}
