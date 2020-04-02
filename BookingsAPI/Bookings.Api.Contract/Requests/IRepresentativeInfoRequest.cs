namespace Bookings.Api.Contract.Requests
{
    public interface IRepresentativeInfoRequest
    {
        /// <summary>
        ///     Participant Organisation
        /// </summary>
        string OrganisationName { get; set; }

        /// <summary>
        ///     Reference
        /// </summary>
        string Reference { get; set; }

        /// <summary>
        ///     Representee
        /// </summary>
        string Representee { get; set; }        
    }
}
