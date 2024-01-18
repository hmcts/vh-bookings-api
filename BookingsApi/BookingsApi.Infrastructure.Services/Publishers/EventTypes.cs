namespace BookingsApi.Infrastructure.Services.Publishers
{
    public enum EventType
    {
        NewParticipantWelcomeEmailEvent,
        ExistingParticipantHearingConfirmationEvent,
        NewParticipantHearingConfirmationEvent,
        CreateConferenceEvent,
        NewParticipantMultidayHearingConfirmationEvent,
        ExistingParticipantMultidayHearingConfirmationEvent,
        ParticipantAddedEvent,
        CreateAndNotifyUserEvent,
        HearingNotificationEvent,
        MultiDayHearingIntegrationEvent,
        HearingNotificationForJudiciaryParticipantEvent,
        ParticipantUpdateEvent
    }
}
