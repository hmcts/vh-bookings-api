using System;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain
{
    public class JudiciaryParticipant : ParticipantBase
    {
        private readonly DateTime _currentUTC = DateTime.UtcNow;
        
        public JudiciaryParticipant(string displayName, JudiciaryPerson judiciaryPerson, JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            Id = Guid.NewGuid();
            DisplayName = displayName;
            JudiciaryPerson = judiciaryPerson;
            HearingRoleCode = hearingRoleCode;
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
        }
        
        protected JudiciaryParticipant()
        {
            Id = Guid.NewGuid();
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
        }
        
        public Guid JudiciaryPersonId { get; private set; }
        public virtual JudiciaryPerson JudiciaryPerson { get; private set; }
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }
        public Guid HearingId { get; private set; }
        public virtual Hearing Hearing { get; private set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
