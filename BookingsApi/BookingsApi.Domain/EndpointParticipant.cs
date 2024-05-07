using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain;

public sealed class EndpointParticipant : TrackableEntity<Guid>
{
    public Guid EndpointId { get; set; }
    public Endpoint Endpoint { get; set; }
    public Guid ParticipantId { get; set; }
    public Participant Participant { get; set; }
    public LinkedParticipantType Type { get; private set; }
    
    public EndpointParticipant(Endpoint endpoint, Participant participant, LinkedParticipantType type)
    {
        Id = Guid.NewGuid();
        Endpoint = endpoint;
        Participant = participant;
        Type = type;
    }
    public EndpointParticipant() {}
}