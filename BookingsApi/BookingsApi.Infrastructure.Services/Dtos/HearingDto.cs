using System;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class HearingDto
    {
        public Guid HearingId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime ScheduledDateTime { get; set;}
        public int ScheduledDuration { get;set; }
        public string CaseType { get; set;}
        public string CaseNumber { get; set;}
        public string CaseName { get; set;}
        public string HearingVenueName { get; set;}
        public bool RecordAudio { get; set; }
    }
}