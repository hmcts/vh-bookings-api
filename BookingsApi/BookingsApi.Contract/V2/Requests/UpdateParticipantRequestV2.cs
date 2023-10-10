using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BookingsApi.Contract.Interfaces.Requests;

namespace BookingsApi.Contract.V2.Requests
{
    public class UpdatePersonDetailsRequestV2
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
    
    public class UpdateParticipantRequestV2 : IUpdateParticipantRequest
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
        ///     Participant first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Participant middle name.
        /// </summary>
        public string MiddleNames { get; set; }

        /// <summary>
        ///     Participant last name.
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        ///     List of linked participants
        /// </summary>
        public List<LinkedParticipantRequestV2> LinkedParticipants { get; set; }
    }
}