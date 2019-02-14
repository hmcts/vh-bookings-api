namespace Bookings.Domain.Enumerations
{
    public enum ParticipantStatusType
    {
        NotSignedIn = 1,
        NotYetJoined = 2,
        Joining = 3,
        UnableToJoin = 4,
        Available = 5,
        InConsultation = 6,
        InHearing = 7,
        Disconnected = 8,
        Unavailable = 9
    }
}
