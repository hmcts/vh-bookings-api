using System.Collections.Generic;

namespace BookingsApi.Domain.Dtos;

public class NewEndpointDto
{
    public string DisplayName { get; set; }
    public string Sip { get; set; }
    public string Pin { get; set; }
    public List<NewEndpointParticipantDto> EndpointParticipants { get; set; } = new ();
}