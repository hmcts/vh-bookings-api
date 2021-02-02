using BookingsApi.Domain;
using Bookings.Infrastructure.Services.Dtos;

namespace Bookings.Infrastructure.Services
{
    public static class EndpointDtoMapper
    {
        public static EndpointDto MapToDto(Endpoint source)
        {
            return new EndpointDto
            {
                DisplayName = source.DisplayName,
                Sip = source.Sip,
                Pin = source.Pin,
                DefenceAdvocateUsername = source.DefenceAdvocate?.Person.Username
            };
        }
    }
}