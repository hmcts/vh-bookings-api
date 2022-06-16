using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingsApi.Contract.Requests
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
    
    public class UpdateParticipantRequest : IRepresentativeInfoRequest
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
    }
}