using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public class JudiciaryParticipant : ParticipantBase
    {
        private readonly DateTime _currentUTC = DateTime.UtcNow;

        public JudiciaryParticipant(string displayName, JudiciaryPerson judiciaryPerson, JudiciaryParticipantHearingRoleCode hearingRoleCode, string optionalContactEmail = null, string optionalContactTelephone = null)
        {
            Id = Guid.NewGuid();
            UpdateDisplayName(displayName);
            JudiciaryPerson = judiciaryPerson;
            UpdateHearingRoleCode(hearingRoleCode);
            ContactEmail = String.IsNullOrWhiteSpace(optionalContactEmail) ? null : optionalContactEmail;
            ContactTelephone =  String.IsNullOrWhiteSpace(optionalContactTelephone) ? null : optionalContactTelephone;
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
        public JudiciaryPerson JudiciaryPerson { get; private set; }
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; private set; }
        public Guid HearingId { get; private set; }
        public virtual Hearing Hearing { get; private set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ContactEmail { get; set; }
        public string ContactTelephone { get; set; }
        
        public string GetEmail() => JudiciaryPerson.IsGeneric 
            ? ContactEmail ?? JudiciaryPerson.Email 
            : JudiciaryPerson.Email;
        public string GetTelephone() => JudiciaryPerson.IsGeneric 
            ? ContactTelephone ?? JudiciaryPerson.WorkPhone 
            : JudiciaryPerson.WorkPhone;
        
        
        public void UpdateDisplayName(string displayName)
        {
            if (displayName == null || displayName.Trim() == string.Empty)
            {
                throw new DomainRuleException(nameof(displayName), "Display name cannot be empty");
            }

            DisplayName = displayName;

            UpdatedDate = _currentUTC;
        }
        
        public void UpdateHearingRoleCode(JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            HearingRoleCode = hearingRoleCode;
            
            UpdatedDate = _currentUTC;
        }
    }
}
