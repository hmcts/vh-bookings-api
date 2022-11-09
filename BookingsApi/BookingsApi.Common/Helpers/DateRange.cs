using System;
namespace BookingsApi.Common.Helpers
{
    public class DateRange
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        
        /// <summary>
        /// Checks whether the current date range falls within another date range.
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        public bool IsInRange(DateRange dateRange)
        {
            return IsInRange(dateRange.StartDate, dateRange.EndDate);
        }

        public bool IsInRange(DateTime startDate, DateTime endDate)
        {
            return StartDate <= endDate && startDate <= EndDate;
        }
    }
}
