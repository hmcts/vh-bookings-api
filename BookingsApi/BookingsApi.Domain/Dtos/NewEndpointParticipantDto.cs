using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain.Dtos;

public class NewEndpointParticipantDto
{
    public string ContactEmail { get; set; }
    public LinkedParticipantType Type { get; set; }
}