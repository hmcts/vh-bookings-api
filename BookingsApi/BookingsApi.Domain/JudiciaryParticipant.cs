using System;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public class JudiciaryParticipant : ParticipantBase
    {
        public JudiciaryParticipant(
            string displayName, 
            JudiciaryPerson judiciaryPerson, 
            JudiciaryParticipantHearingRoleCode hearingRoleCode, 
            string optionalContactEmail = null, 
            string optionalContactTelephone = null) : this()
        {
            UpdateDisplayName(displayName);
            JudiciaryPerson = judiciaryPerson;
            UpdateHearingRoleCode(hearingRoleCode);
            ContactEmail = String.IsNullOrWhiteSpace(optionalContactEmail) ? null : optionalContactEmail;
            ContactTelephone =  String.IsNullOrWhiteSpace(optionalContactTelephone) ? null : optionalContactTelephone;
        }
        
        protected JudiciaryParticipant()
        {
        }
        
#pragma warning disable S1144
        public Guid JudiciaryPersonId { get; private set; }
#pragma warning restore S1144
        public JudiciaryPerson JudiciaryPerson { get; private set; }
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; private set; }
        public Guid HearingId { get; private set; }
        public virtual Hearing Hearing { get; private set; }
        
        /// <summary>
        /// Do not use GETTER for this property. Use GetEmail() instead
        /// </summary>
        public string ContactEmail { get; set; }
        
        /// <summary>
        /// Do not use GETTER for this property. Use GetTelephone() instead
        /// </summary>
        public string ContactTelephone { get; set; }
        
        public int? InterpreterLanguageId { get; protected set; }
        public virtual InterpreterLanguage InterpreterLanguage { get; protected set; }
        public string OtherLanguage { get; set; }
        
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

            UpdatedDate = DateTime.UtcNow;
        }
        
        public void UpdateHearingRoleCode(JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            HearingRoleCode = hearingRoleCode;
            
            UpdatedDate = DateTime.UtcNow;
        }
        
        public void UpdateLanguagePreferences(InterpreterLanguage language, string otherLanguage)
        {
            if (language != null && !string.IsNullOrEmpty(otherLanguage))
            {
                throw new DomainRuleException(nameof(JudiciaryParticipant), DomainRuleErrorMessages.LanguageAndOtherLanguageCannotBeSet);
            }
            InterpreterLanguage = language;
            OtherLanguage = otherLanguage;
            
            UpdatedDate = DateTime.UtcNow;
        }
    }
}
