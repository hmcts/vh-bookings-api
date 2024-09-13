using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain.SpecialMeasure;

using System;

public class Screening : TrackableEntity<Guid>
{
    protected Screening()
    {
        Id = Guid.NewGuid();
        ScreeningEntities = new List<ScreeningEntity>();
    }
    
    public Screening(ScreeningType type, Participant participant) : this()
    {
        Type = type;
        Participant = participant;
    }
    
    public Screening(ScreeningType type, Endpoint endpoint) : this()
    {        
        Type = type;
        Endpoint = endpoint;
    }
    
    public Guid? ParticipantId { get; set; }
    public virtual Participant Participant { get; set; }
    
    public Guid? EndpointId { get; set; }
    public virtual Endpoint Endpoint { get; set; }
    
    public ScreeningType Type { get; set; }
    // public virtual ICollection<Endpoint> EndpointsToScreenFrom { get; private set; }
    // public virtual ICollection<Participant> ParticipantsToScreenFrom { get; private set; }
    public virtual ICollection<ScreeningEntity> ScreeningEntities { get; private set; }

    public void AddParticipant(Participant participant)
    {
        if(ScreeningEntities.Any(x => x.ParticipantId == participant.Id))
        {
            throw new DomainRuleException(nameof(Screening), DomainRuleErrorMessages.ParticipantAlreadyAdded);
        }
        ScreeningEntities.Add(ScreeningEntity.ForParticipant(this, participant));
    }
    
    public void AddEndpoint(Endpoint endpoint)
    {
        if(ScreeningEntities.Any(x => x.EndpointId == endpoint.Id))
        {
            throw new DomainRuleException(nameof(Screening), DomainRuleErrorMessages.EndpointAlreadyAdded);
        }
        ScreeningEntities.Add(ScreeningEntity.ForEndpoint(this, endpoint));
    }
}

public class ScreeningEntity : TrackableEntity<long>
{
    public Guid ScreeningId { get; set; }
    public virtual Screening Screening { get; set; }
    
    public Guid? ParticipantId { get; set; }
    public virtual Participant Participant { get; set; }
    
    public Guid? EndpointId { get; set; }
    public virtual Endpoint Endpoint { get; set; }
    
    private ScreeningEntity()
    {
        
    }
    
    public static ScreeningEntity ForParticipant(Screening screening, Participant participant)
    {
        return new ScreeningEntity
        {
            Screening = screening,
            Participant = participant
        };
    }
    
    public static ScreeningEntity ForEndpoint(Screening screening, Endpoint endpoint)
    {
        return new ScreeningEntity
        {
            Screening = screening,
            Endpoint = endpoint
        };
    }
}