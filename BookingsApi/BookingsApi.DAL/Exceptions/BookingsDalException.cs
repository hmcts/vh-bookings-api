namespace BookingsApi.DAL.Exceptions
{
    public abstract class BookingsDalException : Exception
    {
        protected BookingsDalException(string message) : base(message)
        {
        }
        
        protected BookingsDalException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}