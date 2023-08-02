using System.Collections;

namespace BookingsApi.Contract.V1.Responses
{
    public class BookingsApiHealthResponse
    {
        public BookingsApiHealthResponse()
        {
            DatabaseHealth = new HealthCheck();
            AppVersion = new ApplicationVersion();
        }
        public HealthCheck DatabaseHealth { get; set; }
        public ApplicationVersion AppVersion { get; set; }
    }
    
    public class HealthCheck
    {
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
        public IDictionary Data { get; set; }
    }
    public class ApplicationVersion
    {
        public string FileVersion { get; set; }
        public string InformationVersion { get; set; }
    }
}