using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain;

public class Endpoint : TrackableEntity<Guid>
{
    public string DisplayName { get; set; }
    public string Sip { get; set; }
    public string Pin { get; set; }
    public virtual IList<EndpointParticipant> EndpointParticipants { get; } = new List<EndpointParticipant>();
    public Guid HearingId { get; set; }
    public virtual Hearing Hearing { get; protected set; }
    [Obsolete("This property is only used for EF. Use EndpointParticipants instead")]
    public Participant DefenceAdvocate { get; set; }
    protected Endpoint(){}

    public Endpoint(string displayName, string sip, string pin)
    {
        DisplayName = displayName;
        Sip = sip;
        Pin = pin;
    }
    
    public Endpoint(string displayName, string sip, string pin, params (Participant participant, LinkedParticipantType type)[] participants) : this(displayName, sip, pin)
    {
        if(participants?.Any() ?? false)
            foreach (var (participant, type) in participants)
                LinkParticipantToEndpoint(participant, type);
    }

    public void AssignDefenceAdvocate(Participant defenceAdvocate)
    {
        if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.DefenceAdvocate))
            EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.DefenceAdvocate));

        if(EndpointParticipants.Any(x => x.Participant == defenceAdvocate))
            RemoveLinkedParticipant(defenceAdvocate);
        
        EndpointParticipants.Add(
            new EndpointParticipant(this, defenceAdvocate, LinkedParticipantType.DefenceAdvocate));
    }
    
    public Participant GetDefenceAdvocate()
    {
        return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.DefenceAdvocate)?.Participant;
    }
    
    public void AssignIntermediary(Participant intermediary)
    {
        if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.Intermediary))
            EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.Intermediary));

        if(EndpointParticipants.Any(x => x.Participant == intermediary))
            RemoveLinkedParticipant(intermediary);
        
        EndpointParticipants.Add(
            new EndpointParticipant(this, intermediary, LinkedParticipantType.Intermediary));
    }
    
    public Participant GetIntermediary()
    {
        return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.Intermediary)?.Participant;
    }   
    
    public void AssignRepresentative(Participant representative)
    {
        if(EndpointParticipants.Any(x => x.Type == LinkedParticipantType.Representative))
            EndpointParticipants.Remove(EndpointParticipants.First(x => x.Type == LinkedParticipantType.Representative));

        if(EndpointParticipants.Any(x => x.Participant == representative))
            RemoveLinkedParticipant(representative);
        
        EndpointParticipants.Add(
            new EndpointParticipant(this, representative, LinkedParticipantType.Representative));
    }
    
    public Participant GetRepresentative()
    {
        return EndpointParticipants.FirstOrDefault(x => x.Type == LinkedParticipantType.Representative)?.Participant;
    }
    
    public void RemoveLinkedParticipant(Participant participant)
    {
        var linkedParticipant = EndpointParticipants.FirstOrDefault(x => x.ParticipantId == participant.Id);
        if(linkedParticipant != null)
            EndpointParticipants.Remove(linkedParticipant);
    }
    
    public void UpdateDisplayName(string displayName)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            throw new ArgumentNullException(nameof(displayName));
        }

        DisplayName = displayName;
    }
    
    public void LinkParticipantToEndpoint(Participant participant, LinkedParticipantType type)
    {
        switch (type)
        {
            case LinkedParticipantType.DefenceAdvocate:
                AssignDefenceAdvocate(participant);
                break;
            case LinkedParticipantType.Intermediary:
                AssignIntermediary(participant);
                break;
            case LinkedParticipantType.Representative:
                AssignRepresentative(participant);
                break;
            case LinkedParticipantType.Interpreter:
            default:
                throw new ArgumentException("Invalid participant type for linking participant to endpoint", type.ToString());
        }
    }
}

