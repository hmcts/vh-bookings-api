namespace BookingsApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class EndPointNotFoundException : Exception
    {
        public EndPointNotFoundException(Guid endpointId) : base($"Endpoint {endpointId} does not exist")
        {
        }
    }
}
