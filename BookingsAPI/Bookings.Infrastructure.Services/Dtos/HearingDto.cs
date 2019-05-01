using System;

namespace Bookings.Infrastructure.Services.Dtos
{
    public class HearingDto
    {
        public HearingDto(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration, string caseType,
            string caseNumber, string caseName)
        {
            HearingId = hearingId;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            CaseType = caseType;
            CaseNumber = caseNumber;
            CaseName = caseName;
        }

        public Guid HearingId { get; }
        public DateTime ScheduledDateTime { get; }
        public int ScheduledDuration { get; }
        public string CaseType { get; }
        public string CaseNumber { get; }
        public string CaseName { get; }
    }
}