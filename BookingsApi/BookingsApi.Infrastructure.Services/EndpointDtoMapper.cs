using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
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
                DefenceAdvocateContactEmail = source.DefenceAdvocate?.Person.ContactEmail
            };
        }
    }
}