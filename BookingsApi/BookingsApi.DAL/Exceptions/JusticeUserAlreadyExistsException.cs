namespace BookingsApi.DAL.Exceptions;

[Serializable]
public class JusticeUserAlreadyExistsException : BookingsDalException
{
    public JusticeUserAlreadyExistsException(string username) : base($"A justice user with the username {username} already exists")
    {
    }

    protected JusticeUserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}