namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class EndpointDto
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public string DefenceAdvocateContactEmail { get; set; }
        public ConferenceRole Role { get; set; }
    }

    public enum ConferenceRole
    {
        Host = 1,
        Guest = 2
    }
}