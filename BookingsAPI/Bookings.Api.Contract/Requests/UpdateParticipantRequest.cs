namespace Bookings.Api.Contract.Requests
{
    public class UpdateParticipantRequest
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
        /// House number of an Individual
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// Stree number of an Individual
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

        /// <summary>
        /// Organization name
        /// </summary>
        public string OrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the solicitor reference
        /// </summary>
        public string SolicitorReference { get; set; }

        /// <summary>
        /// Gets or set the representee of a representative
        /// </summary>
        public string Representee { get; set; }
    }
}