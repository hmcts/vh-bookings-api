using System;
using System.Collections.Generic;

namespace Bookings.Api.Contract.Responses
{
    public class HearingSuitabilityAnswerResponse
    {
        /// <summary>
        ///     Participant Id
        /// </summary>
        public Guid ParticipantId { get; set; }

        /// <summary>
        /// Scheduled At
        /// </summary>
        public DateTime ScheduledAt { get; set; }

        /// <summary>
        /// Updated At
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Created At
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// List of answers
        /// </summary>
        public IList<SuitabilityAnswerResponse> Answers { get; set; }


    }
}
