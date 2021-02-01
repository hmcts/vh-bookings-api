using System.Collections.Generic;
using Bookings.Domain;

namespace Bookings.Api.Contract.Requests
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