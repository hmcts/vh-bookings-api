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

        protected Hearing(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue, string hearingRoomName, string otherInformation, string createdBy)
            : this()
        {
            ValidateArguments(scheduledDateTime, scheduledDuration, hearingVenue, hearingType);

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            CaseTypeId = caseType.Id;
            HearingTypeId = hearingType.Id;
            HearingVenueName = hearingVenue.Name;

            Status = BookingStatus.Booked;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            CreatedBy = createdBy;
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
        public BookingStatus Status { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; protected set; }
        public virtual IList<Participant> Participants { get; }
        protected virtual IList<HearingCase> HearingCases { get; set; } = new List<HearingCase>();
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }

        public void CancelHearing()
        {
            Status = BookingStatus.Cancelled;
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

        public Participant AddIndividual(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName)
        {
            if (DoesParticipantExist(person.Username))
            {
                throw new DomainRuleException(nameof(person), "Participant already exists in the hearing");
            }

            Participant participant = new Individual(person, hearingRole, caseRole);
            participant.DisplayName = displayName;
            participant.CreatedBy = CreatedBy;
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            return participant;
        }

        public Participant AddSolicitor(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName,
            string solicitorsReference, string representee)
        {
            if (DoesParticipantExist(person.Username))
            {
                throw new DomainRuleException(nameof(person), "Participant already exists in the hearing");
            }

            Participant participant = new Representative(person, hearingRole, caseRole)
            {
                SolicitorsReference = solicitorsReference,
                Representee = representee
            };

            participant.DisplayName = displayName;
            participant.CreatedBy = CreatedBy;
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            return participant;
        }
        
        public void AddJudge(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName)
        {
            if (DoesParticipantExist(person.Username))
            {
                throw new DomainRuleException(nameof(person), "Judge with given username already exists in the hearing");
            }

            Participant participant = new Judge(person, hearingRole, caseRole);
            participant.DisplayName = displayName;
            participant.CreatedBy = CreatedBy;
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveParticipant(Participant participant)
        {
            if (!DoesParticipantExist(participant.Person.Username))
            {
                throw new DomainRuleException("Participant", "Participant does not exist on the hearing");
            }

            if (GetParticipants().Count < 2)
            {
                throw new DomainRuleException("Participant", "A hearing must have at least one participant");
            }

            var existingParticipant = Participants.Single(x => x.Person.Username == participant.Person.Username);
            Participants.Remove(existingParticipant);
            UpdatedDate = DateTime.UtcNow;
        }

        public virtual IList<Person> GetPersons()
        {
            return Participants.Select(x => x.Person).ToList();
        }
        
        public virtual IList<Participant> GetParticipants()
        {
            return Participants;
        }

        public IList<Case> GetCases()
        {
            return HearingCases.Select(x => x.Case).ToList();
        }

        public virtual void UpdateCase(Case @case)
        {
            //It has been assumed that only one case exists for a given hearing, for now.
            var existingCase = GetCases().FirstOrDefault();
            existingCase.Number = @case.Number;
            existingCase.Name = @case.Name;
        }

        public void UpdateHearingDetails(HearingVenue hearingVenue, DateTime scheduledDateTime, int scheduledDuration, string hearingRoomName, string otherInformation, string updatedBy, List<Case> cases)
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

            if(cases.Any())
            {
                UpdateCase(cases.First());
            }

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.UtcNow;
        }

        private bool DoesParticipantExist(string username)
        {
            return Participants.Any(x => x.Person.Username == username);
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
                _validationFailures.AddFailure("HearingVenue", "HearingVenue must have a valid value");
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

        public void UpdateStatus(BookingStatus newStatus, string updatedBy)
        {
            if(string.IsNullOrEmpty(updatedBy))
            {
                throw new ArgumentNullException(nameof(updatedBy));
            }

            var bookingStatusTransition = new BookingStatusTransition();
            var statusChangedEvent = new StatusChangedEvent(Status, newStatus);

            if (!bookingStatusTransition.IsValid(statusChangedEvent))
            {
                throw new DomainRuleException("BookingStatus", $"Cannot change the booking status from {Status} to {newStatus}");
            }
            
            Status = newStatus;
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}