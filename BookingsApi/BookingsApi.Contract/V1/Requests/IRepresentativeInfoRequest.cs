namespace BookingsApi.Contract.V1.Requests
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
