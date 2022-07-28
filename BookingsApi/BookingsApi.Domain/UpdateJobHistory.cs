namespace BookingsApi.Domain
{
    public class UpdateJobHistory : JobHistory
    {
        public UpdateJobHistory(string jobName, bool isSuccessful)
        {
            JobName = jobName;
            IsSuccessful = isSuccessful;
        }
    }
}