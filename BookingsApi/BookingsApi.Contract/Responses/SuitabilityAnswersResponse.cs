﻿using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class SuitabilityAnswersResponse : PagedCursorBasedResponse
    {
        /// <summary>
        /// Gets or sets a list of participants suitability answers.
        /// </summary>
        public List<ParticipantSuitabilityAnswerResponse> ParticipantSuitabilityAnswerResponse { get; set; }
    }
}
