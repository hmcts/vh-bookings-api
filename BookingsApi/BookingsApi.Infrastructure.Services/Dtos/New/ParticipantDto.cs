using System;
using System.Collections.Generic;

namespace BookingsApi.Infrastructure.Services.Dtos.New
{
    public class ParticipantDto
    {
        public Guid ParticipantId { get; set; }
        public string ContactEmail { get; set; }
        public string DisplayName { get; set; }
        public string HearingRole { get; set; }
        public string UserRole { get; set; }
        public IList<LinkedParticipantDto> LinkedParticipants { get; set; }
        public string ContactEmailForNonEJudJudgeUser { get; set; }
        public string ContactPhoneForNonEJudJudgeUser { get; internal set; }
        public bool SendHearingNotificationIfNew { get; set; }
    }
}