using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain;

public class EndpointLinkedParticipant : TrackableEntity<Guid>
{
    public Guid EndpointId { get; set; }
    public virtual Endpoint Endpoint { get; set; }
    public Guid ParticipantId { get; set; }
    public virtual Participant Participant { get; set; }
    public LinkedParticipantType Type { get; private set; }
    
    public EndpointLinkedParticipant(Endpoint endpoint, Participant participant, LinkedParticipantType type)
    {
        Endpoint = endpoint;
        Participant = participant;
        Type = type;
    }
    public EndpointLinkedParticipant() {}
}