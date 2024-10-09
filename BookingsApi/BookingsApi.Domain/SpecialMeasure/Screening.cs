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

    public ScreeningType Type { get; private set; }
    public virtual ICollection<ScreeningEntity> ScreeningEntities { get; private set; }

    public void UpdateType(ScreeningType type)
    {
        Type = type;
        if (type == ScreeningType.All)
        {
            ScreeningEntities.Clear();
        }
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateScreeningList(List<Participant> participants, List<Endpoint> endpoints)
    {
        ScreeningEntities.Clear();
        foreach (var participant in participants)
        {
            ScreeningEntities.Add(ScreeningEntity.ForParticipant(this, participant));
        }

        foreach (var endpoint in endpoints)
        {
            ScreeningEntities.Add(ScreeningEntity.ForEndpoint(this, endpoint));
        }

        UpdatedDate = DateTime.UtcNow;
    }

    public void RemoveParticipant(Participant participant)
    {
        var screeningEntity = ScreeningEntities.FirstOrDefault(x =>
            x.ParticipantId == participant.Id || x.Participant?.Id == participant.Id);
        if (screeningEntity == null)
        {
            throw new DomainRuleException(nameof(Screening),
                DomainRuleErrorMessages.ParticipantDoesNotExistForScreening);
        }

        ScreeningEntities.Remove(screeningEntity);
        UpdatedDate = DateTime.UtcNow;
    }

    public void RemoveEndpoint(Endpoint endpoint)
    {
        var screeningEntity =
            ScreeningEntities.FirstOrDefault(x => x.EndpointId == endpoint.Id || x.Endpoint?.Id == endpoint.Id);
        if (screeningEntity == null)
        {
            throw new DomainRuleException(nameof(Screening), DomainRuleErrorMessages.EndpointDoesNotExistForScreening);
        }

        ScreeningEntities.Remove(screeningEntity);
        UpdatedDate = DateTime.UtcNow;
    }

    public List<ScreeningEntity> GetEndpoints()
    {
        return ScreeningEntities.Where(x => x.EndpointId != null || x.Endpoint?.Id != null).ToList();
    }

    public List<ScreeningEntity> GetParticipants()
    {
        return ScreeningEntities.Where(x => x.ParticipantId != null || x.Participant?.Id != null).ToList();
    }
}