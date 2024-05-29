using System;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Responses;

public class EndpointParticipantResponse
{
    public Guid ParticipantId { get; set; }
    public LinkedParticipantTypeV2 LinkedParticipantType { get; set; }
}