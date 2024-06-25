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
        HearingNotificationForJudiciaryParticipantEvent,
        HearingNotificationForNewJudicialOfficersEvent,
        JudiciaryParticipantAddedEvent
    }
}
