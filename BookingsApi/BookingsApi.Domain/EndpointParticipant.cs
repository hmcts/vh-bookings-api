using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain;

public sealed class EndpointParticipant : TrackableEntity<Guid>
{
    public Guid EndpointId { get; }
    public Endpoint Endpoint { get; }
    public Guid ParticipantId { get; }
    public Participant Participant { get; }
    public LinkedParticipantType Type { get;}
    
    public EndpointParticipant() { }
    
    public EndpointParticipant(Endpoint endpoint, Participant participant, LinkedParticipantType type)
    {
        EndpointId = endpoint.Id;
        Endpoint = endpoint;
        ParticipantId = participant.Id;
        Participant = participant;
        Type = type;
    }
}