using System;

namespace BookingsApi.Contract.Responses
{
    public class JobHistoryResponse
    {
        public JobHistoryResponse(string jobName, DateTime? lastRunDate, bool isSuccessful)
        {
            JobName = jobName;
            LastRunDate = lastRunDate;
            IsSuccessful = isSuccessful;
        }
        public DateTime? LastRunDate { get; }
        public string JobName { get; }
        public bool IsSuccessful { get; }
    }
}