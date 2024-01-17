using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Domain.Extensions
{
    public static class HearingListExtensions
    {
        public static DateTime ScheduledEndTimeOfLastHearing(this IEnumerable<Hearing> hearingList) =>
            hearingList
                .OrderBy(x => x.ScheduledDateTime)
                .Last()
                .ScheduledEndTime;
    }
}
