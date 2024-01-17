using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Domain.Extensions
{
    public static class HearingListExtensions
    {
        public static DateTime ScheduledEndTimeOfLastHearing(this IEnumerable<Hearing> hearingList)
        {
            var lastHearing = hearingList
                .OrderByDescending(x => x.ScheduledDateTime)
                .First();
            
            return lastHearing.ScheduledEndTime;
        }
    }
}
