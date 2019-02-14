using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Domain.Ddd;
using Bookings.Domain.Enumerations;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;

namespace Bookings.Domain
{
    public abstract class Hearing : AggregateRoot<Guid>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();

        protected Hearing()
        {
            Id = Guid.NewGuid();
            Cases = new List<Case>();
            Participants = new List<Participant>();
            CreatedDate = DateTime.UtcNow;
        }

        protected Hearing(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue)
            : this()
        {
            ValidateArguments(scheduledDateTime, scheduledDuration, hearingVenue, hearingType);

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            CaseTypeId = caseType.Id;
            HearingTypeId = hearingType.Id;
            HearingVenueName = hearingVenue.Name;

            Status = HearingStatus.Created;
        }

        public abstract HearingMediumType HearingMediumType { get; protected set; }
        public virtual HearingVenue HearingVenue { get; protected set; }
        public string HearingVenueName { get; set; }
        public int CaseTypeId { get; set; }
        public virtual CaseType CaseType { get; set; }
        public int HearingTypeId { get; set; }
        public virtual HearingType HearingType { get; protected set; }
        protected virtual IList<Case> Cases { get; set; }
        public DateTime ScheduledDateTime { get; protected set; }
        public int ScheduledDuration { get; protected set; }
        public HearingStatus Status { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; protected set; }
        public virtual IList<Participant> Participants { get; set; }
        protected virtual IList<HearingCase> HearingCases { get; set; } = new List<HearingCase>();

        public void UpdateStatus(Person person, HearingStatus newStatus)
        {
            // TODO: review the state change
//            var hearingStateMachine = new HearingStateMachine();
//            var participant = Participants.Single(x => x.Person.Username == person.Username);
            
//            var statusChangedEvent = new StatusChangedEvent(Status, newStatus, participant.HearingRole);

//            if (!DoesParticipantExist(person) || !hearingStateMachine.ValidateStatusChange(statusChangedEvent))
//            {
//                throw new DomainRuleException("HearingStatus", $"Cannot change the status from {Status} to {newStatus}");
//            }

            Status = newStatus;

            UpdatedDate = DateTime.UtcNow;
        }

        public virtual void AddCase(string number, string name, bool isLeadCase)
        {
            var caseExists = Cases.SingleOrDefault(x => x.Number == number && x.Name == name);
            if (caseExists != null)
            {
                throw new DomainRuleException("Case", "Case already exists for the hearing");
            }
            var newCase = new Case(number, name)
            {
                IsLeadCase = isLeadCase
            };
            HearingCases.Add(new HearingCase { Case = newCase, Hearing = this});
            Cases.Add(newCase);
            
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddCases(IList<Case> cases)
        {
            foreach (var newCase in cases)
            {
                AddCase(newCase.Number, newCase.Name, newCase.IsLeadCase);
            }
        }

        public void AddParticipant(Participant participant)
        {
            if (DoesParticipantExist(participant.Person))
            {
                throw new DomainRuleException("Participant", "Participant already exists on the hearing");
            }
            
            Participants.Add(participant);
            
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveParticipant(Person person)
        {
            if (!DoesParticipantExist(person))
            {
                throw new DomainRuleException("Participant", "Participant does not exist on the hearing");
            }

            if (GetParticipants().Count < 2)
            {
                throw new DomainRuleException("Participant", "A hearing must have at least one participant");
            }

            var existingParticipant = Participants.Single(x => x.Person.Username == person.Username);
            Participants.Remove(existingParticipant);
        }

        public virtual IList<Person> GetPersons()
        {
            return Participants.Select(x => x.Person).ToList();
        }

        public void UpdateCaseType(CaseType caseType, HearingType hearingType)
        {
            if(caseType == null) throw new DomainRuleException(nameof(CaseType), "CaseType cannot be null");
            if(hearingType == null) throw new DomainRuleException(nameof(HearingType), "HearingType cannot be null");
            
            CaseTypeId = caseType.Id;
            CaseType = caseType;

            HearingTypeId = hearingType.Id;
            HearingType = hearingType;
        }

        public bool PersonExists(Person person)
        {
            return Participants.Any(x => x.Person.Username == person.Username);
        }

        
        public virtual IList<Participant> GetParticipants()
        {
            return Participants;
        }

        public virtual Participant GetParticipantByUsername(string username)
        {
            return Participants.SingleOrDefault(x => x.Person.Username == username);
        }

        public virtual Participant GetParticipant(Guid participantId)
        {
            return Participants.SingleOrDefault(x => x.Person.Id == participantId);
        }

        public IList<Case> GetCases()
        {
            return HearingCases.Select(x => x.Case).ToList();
        }

        public HearingStatus GetHearingStatus()
        {
            return Status;
        }

        public void UpdateHearingDetails(HearingVenue hearingVenue, DateTime scheduledDateTime, int scheduledDuration)
        {
            ValidateScheduledDate(scheduledDateTime);

            if (scheduledDuration <= 0)
            {
                _validationFailures.AddFailure("ScheduledDuration", "ScheduledDuration is not a valid value");
            }

            if (hearingVenue == null || hearingVenue.Id <= 0)
            {
                _validationFailures.AddFailure("Venue", "Venue must have a valid value");
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }

            if(hearingVenue != null)
            {
                HearingVenue = hearingVenue;
                HearingVenueName = hearingVenue.Name;
            }
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
        }

        private bool DoesParticipantExist(Person person)
        {
            return Participants.Any(x => x.Person.Username == person.Username);
        }

        public void AddParticipants(IEnumerable<Participant> participants)
        {
            participants.ToList().ForEach(x => Participants.Add(x));
        }

        private void ValidateArguments(DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue,
            HearingType hearingType)
        {
            ValidateScheduledDate(scheduledDateTime);

            if (scheduledDuration <= 0)
            {
                _validationFailures.AddFailure("ScheduledDuration", "ScheduledDuration is not a valid value");
            }
          
            if (hearingVenue == null || hearingVenue.Id <=0)
            {
                _validationFailures.AddFailure("Court", "Court must have a valid value");
            }
            if (hearingType == null || hearingType.Id <= 0)
            {
                _validationFailures.AddFailure("HearingType", "HearingType must have a valid value");
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
        }

        private void ValidateScheduledDate(DateTime scheduledDateTime)
        {
            if (scheduledDateTime == default(DateTime))
            {
                _validationFailures.AddFailure(nameof(ScheduledDateTime), "ScheduledDateTime is not a valid value");
            }
            if (scheduledDateTime.Date.CompareTo(DateTime.UtcNow.Date) < 0)
            {
                _validationFailures.AddFailure(nameof(ScheduledDateTime), "Schedule datetime cannot be set in the past");
            }
        }
    }
}