using System;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;

namespace Bookings.Infrastructure.Services.Dtos
{
    public class ParticipantDto
    {
        public ParticipantDto(Guid participantId, string fullname, string username, string displayName,
            string hearingRole, string userRole, CaseRoleGroup caseGroupType, string representee)
        {
            ParticipantId = participantId;
            Fullname = fullname;
            Username = username;
            DisplayName = displayName;
            HearingRole = hearingRole;
            UserRole = userRole;
            CaseGroupType = caseGroupType;
            Representee = representee;
        }

        public Guid ParticipantId { get; }
        public string Fullname { get; }
        public string Username { get; }
        public string DisplayName { get; }
        public string HearingRole { get; }
        public string UserRole { get; }
        public CaseRoleGroup CaseGroupType { get; }
        public string Representee { get; }
    }
}