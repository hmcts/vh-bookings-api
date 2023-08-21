using System;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain
{
    public class JudiciaryParticipant : TrackableEntity<Guid>
    {
        public JudiciaryParticipant(string displayName, JudiciaryPerson judiciaryPerson, HearingRoleCode hearingRoleCode)
        {
            Id = Guid.NewGuid();
            DisplayName = displayName;
            JudiciaryPerson = judiciaryPerson;
            HearingRoleCode = hearingRoleCode;
        }
        
        public string DisplayName { get; private set; }
        public Guid JudiciaryPersonId { get; private set; }
        public virtual JudiciaryPerson JudiciaryPerson { get; private set; }
        public HearingRoleCode HearingRoleCode { get; private set; }
        public Guid HearingId { get; private set; }
        public virtual Hearing Hearing { get; private set; }
    }
}
