using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain.Dtos;

public class NewEndpointParticipantDto(string contactEmail, LinkedParticipantType type)
{
    public string ContactEmail { get; set; } = contactEmail;
    public LinkedParticipantType Type { get; set; } = type;
}