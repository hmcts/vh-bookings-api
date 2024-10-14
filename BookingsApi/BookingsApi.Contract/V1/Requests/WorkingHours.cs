﻿using System;
using System.Text.Json.Serialization;

namespace BookingsApi.Contract.V1.Requests
{
    public class WorkingHours
    {
        public int DayOfWeekId { get; set; }
        public int? EndTimeHour { get; set; }
        public int? EndTimeMinutes { get; set; }
        public int? StartTimeHour { get; set; }
        public int? StartTimeMinutes { get; set; }

        [JsonIgnore]
        public TimeSpan? StartTime => StartTimeHour == null || StartTimeMinutes == null ? (TimeSpan?)null : new TimeSpan((int)StartTimeHour, (int)StartTimeMinutes, 0);

        [JsonIgnore]
        public TimeSpan? EndTime => EndTimeHour == null || EndTimeMinutes == null ? (TimeSpan?)null : new TimeSpan((int)EndTimeHour, (int)EndTimeMinutes, 0);

        public WorkingHours(int dayOfWeekId, int? startTimeHour, int? startTimeMinutes, int? endTimeHour, int? endTimeMinutes)
        {
            DayOfWeekId = dayOfWeekId;
            EndTimeHour = endTimeHour;
            EndTimeMinutes = endTimeMinutes;
            StartTimeHour = startTimeHour;
            StartTimeMinutes = startTimeMinutes;
        }
    }
}