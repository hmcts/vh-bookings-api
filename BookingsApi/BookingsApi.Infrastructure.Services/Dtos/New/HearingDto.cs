using System;

namespace BookingsApi.Infrastructure.Services.Dtos.New
{
    public class HearingDto
    {
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int ScheduledDuration { get; set; }
        public bool RecordAudio { get; set; }
    }
}
