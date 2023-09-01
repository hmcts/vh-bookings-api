using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Responses
{
    public class PersonSuitabilityAnswerResponse
    {
        public Guid HearingId { get; set; }
        public Guid ParticipantId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool QuestionnaireNotRequired { get; set; }
        public IList<SuitabilityAnswerResponse> Answers { get; set; }
    }
}
