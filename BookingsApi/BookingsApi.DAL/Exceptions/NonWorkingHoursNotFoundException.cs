namespace BookingsApi.DAL.Exceptions
{
    #pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class NonWorkingHoursNotFoundException : EntityNotFoundException
    {
        public NonWorkingHoursNotFoundException(long id) : base($"Non working hour {id} does not exist")
        {
        }
    }
}
