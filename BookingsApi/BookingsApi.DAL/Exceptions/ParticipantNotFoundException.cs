namespace BookingsApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly

    public class ParticipantNotFoundException : EntityNotFoundException
    {
        public ParticipantNotFoundException(Guid participantId) : base($"Participant {participantId} does not exist")
        {
        }
    }
}
