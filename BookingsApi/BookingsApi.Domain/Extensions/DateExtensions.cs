using System;

namespace BookingsApi.Domain.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToGmt(this DateTime date)
        {
            var dateUtc = date.ToUniversalTime();
            
            var britishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(dateUtc, britishTimeZone);
        }
        public static DateTime TrimMilliseconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }

    }
}
