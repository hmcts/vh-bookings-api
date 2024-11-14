namespace BookingsApi.DAL.Exceptions;

public class ParticipantNotFoundException(Guid participantId)
    : EntityNotFoundException($"Participant {participantId} does not exist");