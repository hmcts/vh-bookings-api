using System;

namespace BookingsApi.Common
{
    public static class DateExtensions
    {
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
        
        public static DateTime GetNextWorkingDay(this DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (IsWeekend(date));
            return date;
        }
        
        public static DateTime GetNextWeekendDay(this DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (!IsWeekend(date));
            return date;
        }
        
        public static DateTime ToGmt(this DateTime date)
        {
            var dateUtc = date.ToUniversalTime();
            
            var britishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, britishTimeZone);
        }
    }
}