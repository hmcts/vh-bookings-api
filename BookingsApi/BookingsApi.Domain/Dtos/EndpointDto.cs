using System.Collections.Generic;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain.Dtos;

public class EndpointDto
{
    public string DisplayName { get; set; }
    public string Sip { get; set; }
    public string Pin { get; set; }
    public List<EndpointParticipantDto> EndpointParticipants { get; set; } 
}

public class EndpointParticipantDto
{
    public string ContactEmail { get; set; }
    public LinkedParticipantType Type { get; set; }
}