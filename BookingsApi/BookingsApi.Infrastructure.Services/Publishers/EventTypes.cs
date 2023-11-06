namespace BookingsApi.Infrastructure.Services.Publishers
{
    public enum EventType
    {
        WelcomeMessageForNewParticipantEvent,
        HearingConfirmationForExistingParticipantEvent,
        HearingConfirmationForNewParticipantEvent,
        CreateConferenceEvent,
        MultidayHearingConfirmationforNewParticipantEvent,
        MultidayHearingConfirmationforExistingParticipantEvent,
        ParticipantAddedEvent
    }
}
