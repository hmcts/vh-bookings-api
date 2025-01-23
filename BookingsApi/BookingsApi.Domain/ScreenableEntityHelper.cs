using System;
using System.Collections.Generic;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.Domain;

public static class ScreenableEntityHelper
{
    public static void AssignScreening(
        IScreenableEntity entity, 
        ScreeningType type, 
        List<Participant> participants, 
        List<Endpoint> endpoints)
    {
        var originalScreeningUpdatedDate = entity.Screening?.UpdatedDate;
        
        ScreeningHelper.AssignScreening(entity, type, participants, endpoints);

        var hasChanged = entity.Screening?.UpdatedDate != originalScreeningUpdatedDate;
        if (!hasChanged) return;
        
        if (entity is ITrackable trackableEntity)
            trackableEntity.UpdatedDate = DateTime.UtcNow;
    }

    public static void RemoveScreening(IScreenableEntity entity)
    {
        ScreeningHelper.RemoveScreening(entity);
        
        if (entity is ITrackable trackableEntity)
            trackableEntity.UpdatedDate = DateTime.UtcNow;
    }
}