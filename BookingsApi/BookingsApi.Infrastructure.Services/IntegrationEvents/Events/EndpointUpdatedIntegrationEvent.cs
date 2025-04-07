using System;
using System.Collections.Generic;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events;

public class EndpointUpdatedIntegrationEvent(
    Guid hearingId,
    string sip,
    string displayName,
    List<string> participantsLinked,
    ConferenceRole role)
    : IIntegrationEvent
{
    public Guid HearingId { get; } = hearingId;
    public string Sip { get; } = sip;
    public string DisplayName { get; } = displayName;
    public List<string> ParticipantsLinked { get; } = participantsLinked;
    public ConferenceRole Role { get; } = role;
}