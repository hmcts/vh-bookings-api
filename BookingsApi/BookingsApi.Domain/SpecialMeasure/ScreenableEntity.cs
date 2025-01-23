using System;
using System.Collections.Generic;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.Domain.SpecialMeasure;

public interface IScreenableEntity
{
    Guid? ScreeningId { get; internal set; }
    Screening Screening { get; internal set; }
}

public static class ScreeningHelper
{
    public static void AssignScreening(IScreenableEntity entity, ScreeningType type, List<Participant> participants, List<Endpoint> endpoints)
    {
        var originalScreeningUpdatedDate = entity.Screening?.UpdatedDate;
        
        var screening = entity.Screening;
        if (screening == null)
        {
            screening = entity switch
            {
                Participant participant => new Screening(type, participant),
                Endpoint endpoint => new Screening(type, endpoint),
                _ => throw new InvalidOperationException($"Screenable Entity not supported, {entity.GetType()}")
            };
            entity.Screening = screening;
            entity.ScreeningId = screening.Id;
            screening.UpdateScreeningList(participants, endpoints);
        }
        else
        {
            screening.UpdateType(type);
            screening.UpdateScreeningList(participants, endpoints);
        }
        
        var hasChanged = entity.Screening?.UpdatedDate != originalScreeningUpdatedDate;
        if (!hasChanged) return;
        
        if (entity is ITrackable trackableEntity)
            trackableEntity.UpdatedDate = DateTime.UtcNow;
    }
    
    public static void RemoveScreening(IScreenableEntity entity)
    {
        if (entity.Screening != null)
        {
            entity.Screening.ScreeningEntities.Clear();
            entity.Screening = null;
            entity.ScreeningId = null;
        }
        
        if (entity is ITrackable trackableEntity)
            trackableEntity.UpdatedDate = DateTime.UtcNow;
    }
}