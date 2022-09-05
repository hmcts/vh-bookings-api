﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Domain
{
    [ExcludeFromCodeCoverage]
    public class VhoNonAvailability : TrackableEntity<long>
    {
        public Guid JusticeUserId { get; set; }
        public JusticeUser JusticeUser { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string CreatedBy { get; set; }
    }
}