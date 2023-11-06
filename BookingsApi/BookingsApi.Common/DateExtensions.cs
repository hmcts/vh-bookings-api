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
        public static DateTime TrimMilliseconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }
    }
}