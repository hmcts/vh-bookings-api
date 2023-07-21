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
    }
}
