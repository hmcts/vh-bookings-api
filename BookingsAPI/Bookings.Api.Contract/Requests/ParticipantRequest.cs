namespace Bookings.Api.Contract.Requests
{
    public class ParticipantRequest : IRepresentativeInfoRequest
    {
        /// <summary>
        ///     Participant Title.
        /// </summary>
        public string Title { get; set; }

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
        ///     Participant Contact Email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        ///     Participant Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }
        
        /// <summary>
        ///     Participant Username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        ///     Participant Display Name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        ///     The name of the participant's case role
        /// </summary>
        public string CaseRoleName { get; set; }
        
        /// <summary>
        ///     The name of the participant's hearing role
        /// </summary>
        public string HearingRoleName { get; set; }

        /// <summary>
        /// The reference for a representative participant
        /// </summary>
        public string Reference { get; set; }
        
        /// <summary>
        /// The representee of a representative
        /// </summary>
        public string Representee { get; set; }
        
          /// <summary>
        /// The organisation name
        /// </summary>
        public string OrganisationName { get; set; }
    }
}