using System;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class HearingConfirmationForParticipantDto
    {
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseName { get; set; }
        public string CaseNumber { get; set; }
        public Guid ParticipnatId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        public string UserRole { get; set; }
        public string Username { get; set; }
        public string Representee { get; set; }
    }
}
