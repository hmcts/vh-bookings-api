namespace BookingsApi.DAL.Exceptions;
public class EndPointNotFoundException(Guid endpointId)
    : EntityNotFoundException($"Endpoint {endpointId} does not exist");