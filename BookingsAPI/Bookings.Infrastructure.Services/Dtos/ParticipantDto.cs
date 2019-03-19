using System;
using Bookings.Domain.Enumerations;

namespace Bookings.Infrastructure.Services.Dtos
{
    public class ParticipantDto
    {
        public ParticipantDto(Guid participantId, string fullname, string username, string displayName,
            string hearingRole, CaseRoleGroup caseGroupType)
        {
            ParticipantId = participantId;
            Fullname = fullname;
            Username = username;
            DisplayName = displayName;
            HearingRole = hearingRole;
            CaseGroupType = caseGroupType;
        }

        public Guid ParticipantId { get; }
        public string Fullname { get; }
        public string Username { get; }
        public string DisplayName { get; }
        public string HearingRole { get; }
        public CaseRoleGroup CaseGroupType { get; }
    }
}