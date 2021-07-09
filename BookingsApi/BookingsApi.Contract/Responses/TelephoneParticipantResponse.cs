using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class TelephoneParticipantResponse
    {
        /// <summary>
        ///     Participant Id
        /// </summary>
        public Guid Id { get; set; } 
        
        /// <summary>
        ///     The name of the participant's case role
        /// </summary>
        public string CaseRoleName { get; set; }
        
        /// <summary>
        ///     The name of the participant's hearing role
        /// </summary>
        public string HearingRoleName { get; set; }

        /// <summary>
        ///     Participant first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Participant last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        ///     Participant contact email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        ///     Participant telephone number
        /// </summary>
        public string TelephoneNumber { get; set; } 
        
        /// <summary>
        ///     Participant telephone number
        /// </summary>
        public string MobileNumber { get; set; }
        
        /// <summary>
        ///     Gets or sets the person name that Representative represents.
        /// </summary>
        public string Representee { get; set; }
        
        /// <summary>
        ///     The participant linked to this participant response
        /// </summary>
        public List<LinkedParticipantResponse> LinkedParticipants { get; set; }
    }
}