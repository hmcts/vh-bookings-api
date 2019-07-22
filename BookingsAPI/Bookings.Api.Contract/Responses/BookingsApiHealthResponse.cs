using System.Collections;

namespace Bookings.Api.Contract.Responses
{
    public class BookingsApiHealthResponse
    {
        public BookingsApiHealthResponse()
        {
            DatabaseHealth = new HealthCheck();
        }
        public HealthCheck DatabaseHealth { get; set; }
    }
    
    public class HealthCheck
    {
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
        public IDictionary Data { get; set; }
    }
}