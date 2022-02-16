using System;

namespace BookingsApi.Domain
{
    public class UpdateJobHistory : JobHistory
    {
        public void UpdateLastRunDate()
        {
            LastRunDate = DateTime.UtcNow;
        }
    }
}