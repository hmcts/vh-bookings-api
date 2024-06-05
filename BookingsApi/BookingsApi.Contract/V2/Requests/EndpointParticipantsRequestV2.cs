using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Requests;

public class EndpointParticipantsRequestV2
{
    public string ContactEmail { get; set; }      
    public LinkedParticipantTypeV2 Type { get; set; }
}