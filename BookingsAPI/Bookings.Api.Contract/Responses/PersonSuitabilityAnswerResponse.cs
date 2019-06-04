using System;
using System.Collections.Generic;

namespace Bookings.Api.Contract.Responses
{
    public class PersonSuitabilityAnswerResponse
    {
        /// <summary>
        ///     Hearing Id
        /// </summary>
        public Guid HearingId { get; set; }

        /// <summary>
        ///     Participant Id
        /// </summary>
        public Guid ParticipantId { get; set; }

        /// <summary>
        /// Scheduled time of the hearing
        /// </summary>
        public DateTime ScheduledAt { get; set; }

        /// <summary>
        /// Created time of the suitability answers
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// List of answers
        /// </summary>
        public IList<SuitabilityAnswerResponse> Answers { get; set; }
    }
}
