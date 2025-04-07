using System.Collections.Generic;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class EndpointDto
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public List<string> ParticipantsLinked { get; set; } = new();
        public ConferenceRole Role { get; set; }
    }

    public enum ConferenceRole
    {
        Host = 1,
        Guest = 2
    }
}