using System.ComponentModel.DataAnnotations;
using BookingsApi.Contract.Interfaces.Requests;

namespace BookingsApi.Contract.V2.Requests
{
    public class ParticipantRequestV2 : IParticipantRequest
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
        ///
        [StringLength(255, ErrorMessage = "Display name max length is 255 characters")]
        [RegularExpression("^([-A-Za-z0-9 ',._])*$")]
        public string DisplayName { get; set; }

        /// <summary>
        ///     The name of the participant's case role (optional)
        /// </summary>
        public string CaseRoleName { get; set; }

        /// <summary>
        ///     The name of the participant's hearing role
        /// </summary>
        public string HearingRoleName { get; set; }

        /// <summary>
        ///     The code of the participant's hearing role
        /// </summary>
        public string HearingRoleCode { get; set; }

        /// <summary>
        ///     The representee of a representative
        /// </summary>
        public string Representee { get; set; }

        /// <summary>
        ///     The organisation name
        /// </summary>
        public string OrganisationName { get; set; }
    }
}
    