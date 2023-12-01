using System;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class WelcomeEmailDto
    {
        public Guid HearingId { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public Guid ParticipnatId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string UserRole { get; set; }
    }
}
