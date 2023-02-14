using System;
using System.Collections.Generic;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

public class UserAllocatedToHearingIntegrationEvent : IIntegrationEvent
{
    public List<Guid> HearingIds { get; }
    public string CsoUsername { get; }
        
    public UserAllocatedToHearingIntegrationEvent(List<Guid> hearingIds, string csoUsername)
    {
        HearingIds = hearingIds;
        CsoUsername = csoUsername;
    }
}