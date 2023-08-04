namespace BookingsApi.Contract.Interfaces.Requests
{
    public interface IRepresentativeInfoRequest
    {
        /// <summary>
        ///     Participant Organisation
        /// </summary>
        string OrganisationName { get; set; }

        /// <summary>
        ///     Representee
        /// </summary>
        string Representee { get; set; }        
    }
}
