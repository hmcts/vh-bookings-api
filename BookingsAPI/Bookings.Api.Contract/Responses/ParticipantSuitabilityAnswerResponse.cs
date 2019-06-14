using System;
using System.Collections.Generic;

namespace Bookings.Api.Contract.Responses
{
    public class ParticipantSuitabilityAnswerResponse
    {
        /// <summary>
        ///     Gets or sets the participant ID
        /// </summary>
        public Guid ParticipantId { get; set; }

        /// <summary>
        ///     Gets or sets the hearing case number
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        ///     Gets or sets the participant hearing role
        /// </summary>
        public string HearingRole { get; set; }

        /// <summary>
        ///     Gets or sets the participant title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the participant first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the participant last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Last updated date and time of the suitability answers
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        ///     Gets or sets the name of represented person
        /// </summary>
        public string Representee { get; set; }

        /// <summary>
        ///     List of answers
        /// </summary>
        public IList<SuitabilityAnswerResponse> Answers { get; set; }
    }
}
