namespace BookingsApi.DAL.Exceptions;

public class HearingNotFoundException(Guid hearingId) : EntityNotFoundException($"Hearing {hearingId} does not exist");