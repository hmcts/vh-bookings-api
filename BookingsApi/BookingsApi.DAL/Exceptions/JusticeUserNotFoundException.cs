namespace BookingsApi.DAL.Exceptions
{
    [Serializable]
    public class JusticeUserNotFoundException : BookingsDalException
    {
        public JusticeUserNotFoundException(Guid id) : base($"Justice user with id {id} not found")
        {
        }
        
        public JusticeUserNotFoundException(string username) : base($"Justice user with username {username} not found")
        {
        }

        protected JusticeUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
