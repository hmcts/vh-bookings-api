namespace BookingsApi.DAL.Queries.BaseQueries
{
    public static class JobHistory
    {
        public static DateTime? GetLastRunDate(this BookingsDbContext context, string jobName) => context.JobHistory
            .Where(e => e.JobName == jobName && e.IsSuccessful)
            .OrderByDescending(e => e.LastRunDate)
            .FirstOrDefault()?.LastRunDate;
    }
}