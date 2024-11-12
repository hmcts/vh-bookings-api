using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain;

public sealed class LinkedParticipant  : TrackableEntity<Guid>
{
    public Guid ParticipantId { get; private set; }
    public Participant Participant { get; set; }
    public Guid LinkedId { get; private set; }
    public Participant Linked { get; set; }
    public LinkedParticipantType Type { get; private set; }
        
    public LinkedParticipant(Guid participantId, Guid linkedId, LinkedParticipantType type)
    {
        Id = Guid.NewGuid();
        ParticipantId = participantId;
        LinkedId = linkedId;
        Type = type;
    }
}