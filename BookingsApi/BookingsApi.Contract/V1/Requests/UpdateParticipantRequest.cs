using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookingsApi.Contract.Interfaces.Requests;

namespace BookingsApi.Contract.V1.Requests
{
    public class UpdatePersonDetailsRequest
    {
        /// <summary>
        ///     Participant first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Participant last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        ///     Participant Username
        /// </summary>
        public string Username { get; set; }
    }
    
    public class UpdateParticipantRequest : IUpdateParticipantRequest
    {
        /// <summary>
        ///     Participant Id.
        /// </summary>
        public Guid ParticipantId { get; set; }

        /// <summary>
        ///     Participant Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Participant Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }
        
        /// <summary>
        ///     Participant Contact email
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        ///     Participant Display Name
        /// </summary>
        /// 
        [StringLength(255, ErrorMessage = "Display name max length is 255 characters")]
        [RegularExpression("^([-A-Za-z0-9 ',._])*$")]
        public string DisplayName { get; set; }

        /// <summary>
        ///     Participant Organisation
        /// </summary>
        public string OrganisationName { get; set; }

        /// <summary>
        ///     Representee
        /// </summary>
        public string Representee { get; set; }
        
        /// <summary>
        ///     List of linked participants
        /// </summary>
        public List<LinkedParticipantRequest> LinkedParticipants { get; set; }
        
        /// <summary>
        /// First Name of the participant
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Middle Name of the participant
        /// </summary>
        public string MiddleName { get; set; }
        
        /// <summary>
        /// Last Name of the participant
        /// </summary>
        public string LastName { get; set; }
    }
}