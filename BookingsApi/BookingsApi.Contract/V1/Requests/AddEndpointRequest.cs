namespace BookingsApi.Contract.V1.Requests
{
    public class AddEndpointRequest : EndpointRequest
    {
        /// <summary>
        ///     Optional name of the user who made the change, uses a default if not provided
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
