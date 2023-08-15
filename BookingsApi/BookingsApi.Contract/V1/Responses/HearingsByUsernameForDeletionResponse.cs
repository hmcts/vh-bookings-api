using System;

namespace BookingsApi.Contract.V1.Responses
{
    public class HearingsByUsernameForDeletionResponse
    {
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public string Venue { get; set; }
    }
}