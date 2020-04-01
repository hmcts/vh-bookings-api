using System;

namespace Bookings.Api.Contract.Responses
{
    public class ParticipantResponse
    {
        /// <summary>
        ///     Participant Id
        /// </summary>
        public Guid Id { get; set; } 
        
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
        ///     The name of the participant's user role
        /// </summary>
        public string UserRoleName { get; set; }
        
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
        ///     Participant contact email
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        ///     Participant telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        ///     Participant username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Gets or sets the reference
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the organisation name.
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// Gets or sets the person name that Representative represents.
        /// </summary>
        public string Representee { get; set; }

        /// <summary>
        /// House number of an Individual
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// Street number of an Individual
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Postcode of an Individual
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// City/Town of an Individual
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// County of an Individual
        /// </summary>
        public string County { get; set; }
    }
}