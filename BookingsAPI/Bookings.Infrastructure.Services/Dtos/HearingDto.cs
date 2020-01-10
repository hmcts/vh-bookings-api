using System;

namespace Bookings.Infrastructure.Services.Dtos
{
    public class HearingDto
    {
        public HearingDto(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration, string caseType,
            string caseNumber, string caseName, string hearingVenueName)
        {
            HearingId = hearingId;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            CaseType = caseType;
            CaseNumber = caseNumber;
            CaseName = caseName;
            HearingVenueName = hearingVenueName;
        }

        public Guid HearingId { get; }
        public DateTime ScheduledDateTime { get; }
        public int ScheduledDuration { get; }
        public string CaseType { get; }
        public string CaseNumber { get; }
        public string CaseName { get; }
        public string HearingVenueName { get; set; }
    }
}