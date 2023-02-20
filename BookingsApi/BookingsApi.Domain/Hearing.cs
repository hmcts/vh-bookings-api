using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public abstract class Hearing : AggregateRoot<Guid>
    {
        private readonly ValidationFailures _validationFailures = new ValidationFailures();
        private readonly DateTime _currentUTC = DateTime.UtcNow;
        private bool _isFirstDayOfMultiDayHearing;

        protected Hearing()
        {
            Id = Guid.NewGuid();
            Cases = new List<Case>();
            Participants = new List<Participant>();
            CreatedDate = _currentUTC;
            UpdatedDate = _currentUTC;
            HearingCases = new List<HearingCase>();
            Endpoints = new List<Endpoint>();
            Allocations = new List<Allocation>();
        }

        protected Hearing(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue hearingVenue, string hearingRoomName,
            string otherInformation, string createdBy, bool questionnaireNotRequired, 
            bool audioRecordingRequired, string cancelReason)
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
            QuestionnaireNotRequired = questionnaireNotRequired;
            AudioRecordingRequired = audioRecordingRequired;
            CancelReason = cancelReason;
        }

        public abstract HearingMediumType HearingMediumType { get; protected set; }
        public virtual HearingVenue HearingVenue { get; protected set; }
        public virtual string HearingVenueName { get; set; }
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
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; protected set; }
        public virtual IList<Participant> Participants { get; }
        public virtual IList<Endpoint> Endpoints { get; }
        public virtual IList<HearingCase> HearingCases { get; set; }
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }
        public bool QuestionnaireNotRequired { get; set; }
        public bool AudioRecordingRequired { get; set; }
        public string CancelReason { get; set; }
        public Guid? SourceId { get; set; }

        // Ideally, the domain object would implement the clone method and so this change is a work around.
        public bool IsFirstDayOfMultiDayHearing
        {
            get => _isFirstDayOfMultiDayHearing;
            set
            {
                _isFirstDayOfMultiDayHearing = value;
                if (_isFirstDayOfMultiDayHearing)
                {
                    SourceId = Id;
                }
            }
        }

        public DateTime ScheduledEndTime => ScheduledDateTime.AddMinutes(ScheduledDuration);
        public virtual IList<Allocation> Allocations { get; protected set; }
        public virtual JusticeUser AllocatedTo => Allocations?.FirstOrDefault()?.JusticeUser;

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
            HearingCases.Add(new HearingCase { Case = newCase, Hearing = this });
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

        public void AddEndpoint(Endpoint endpoint)
        {
            if (DoesEndpointExist(endpoint.Sip))
            {
                throw new DomainRuleException(nameof(endpoint), "Endpoint already exists in the hearing");
            }

            Participant defenceAdvocate = null;

            if (endpoint.DefenceAdvocate != null)
            {
                defenceAdvocate = Participants.Single(x => x.Id == endpoint.DefenceAdvocate.Id);
            }

            Endpoints.Add(new Endpoint(endpoint.DisplayName, endpoint.Sip, endpoint.Pin, defenceAdvocate));
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddEndpoints(List<Endpoint> endpoints)
        {
            endpoints.ForEach(AddEndpoint); 
        }

        public Participant AddIndividual(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName)
        {
            if (DoesParticipantExistByContactEmail(person.ContactEmail))
            {
                throw new DomainRuleException(nameof(person), "Participant already exists in the hearing");
            }

            Participant participant = new Individual(person, hearingRole, caseRole)
            {
                DisplayName = displayName,
                CreatedBy = CreatedBy
            };
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            return participant;
        }

        public Participant AddRepresentative(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName,
            string representee)
        {
            if (DoesParticipantExistByContactEmail(person.ContactEmail))
            {
                throw new DomainRuleException(nameof(person), "Participant already exists in the hearing");
            }

            Participant participant = new Representative(person, hearingRole, caseRole)
            {
                Representee = representee
            };

            participant.DisplayName = displayName;
            participant.CreatedBy = CreatedBy;
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            return participant;
        }

        public Participant AddJudge(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName)
        {
            if(!string.Equals(hearingRole.Name, "Judge", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new DomainRuleException(nameof(hearingRole), "Hearing role should be Judge");
            }

            if (DoesParticipantExistByUsername(person.Username))
            {
                throw new DomainRuleException(nameof(person), "Judge with given username already exists in the hearing");
            }

            if(Participants.Any(x => x.HearingRole == hearingRole))
            {
                throw new DomainRuleException(nameof(person), "A participant with Judge role already exists in the hearing");
            }

            Participant participant = new Judge(person, hearingRole, caseRole)
            {
                DisplayName = displayName,
                CreatedBy = CreatedBy
            };
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            return participant;
        }

        public Participant AddStaffMember(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName)
        {
            if (DoesParticipantExistByUsername(person.Username))
            {
                throw new DomainRuleException(nameof(person), "Staff Member with given username already exists in the hearing");
            }

            Participant participant = new StaffMember(person, hearingRole, caseRole)
            {
                DisplayName = displayName,
                CreatedBy = CreatedBy
            };
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            return participant;
        }
        
        public Participant AddJudicialOfficeHolder(Person person, HearingRole hearingRole, CaseRole caseRole, string displayName)
        {
            if (DoesParticipantExistByContactEmail(person.ContactEmail))
            {
                throw new DomainRuleException(nameof(person), "Judicial office holder already exists in the hearing");
            }

            var participant = new JudicialOfficeHolder(person, hearingRole, caseRole)
            {
                DisplayName = displayName
            };
            Participants.Add(participant);
            UpdatedDate = DateTime.Now;
            return participant;
        }

        public bool HasHost => GetParticipants().Any(x => x.HearingRole.Name == "Judge" || x.HearingRole.Name == "Staff Member");

        public void ValidateHostCount()
        {
            if (!HasHost)
            {
                throw new DomainRuleException("Host", "A hearing must have at least one host");
            }
        }

        public void RemoveParticipant(Participant participant, bool validateParticipantCount=true)
        {
            if (!DoesParticipantExistByContactEmail(participant.Person.ContactEmail))
            {
                throw new DomainRuleException("Participant", "Participant does not exist on the hearing");
            }

            if (validateParticipantCount) ValidateHostCount();

            var existingParticipant = Participants.Single(x => x.Person.ContactEmail == participant.Person.ContactEmail);
            var endpoint = Endpoints.SingleOrDefault(e => e.DefenceAdvocate != null && e.DefenceAdvocate.Id == participant.Id);
            if (endpoint != null)
            {
                endpoint.AssignDefenceAdvocate(null);
            }
            
            participant.LinkedParticipants.Clear();

            Participants.Remove(existingParticipant);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveParticipantById(Guid participantId, bool validateParticipantCount=true)
        {
            var participant = GetParticipants().Single(x => x.Id == participantId);
            RemoveParticipant(participant, validateParticipantCount);
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

        public IList<Endpoint> GetEndpoints()
        {
            return Endpoints;
        }

        public void RemoveEndpoint(Endpoint endpoint)
        {
            endpoint.AssignDefenceAdvocate(null);
            Endpoints.Remove(endpoint);
            UpdatedDate = DateTime.UtcNow;
        }

        public virtual void UpdateCase(Case @case)
        {
            //It has been assumed that only one case exists for a given hearing, for now.
            var existingCase = GetCases().FirstOrDefault();
            if (existingCase == null) return;
            existingCase.Number = @case.Number;
            existingCase.Name = @case.Name;
        }

        public void UpdateHearingDetails(HearingVenue hearingVenue, DateTime scheduledDateTime,
            int scheduledDuration, string hearingRoomName, string otherInformation, string updatedBy,
            List<Case> cases, bool questionnaireNotRequired, bool audioRecordingRequired)
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

            if (hearingVenue != null)
            {
                HearingVenue = hearingVenue;
                HearingVenueName = hearingVenue.Name;
            }

            if (cases.Any())
            {
                UpdateCase(cases.First());
            }

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;

            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.UtcNow;
            QuestionnaireNotRequired = questionnaireNotRequired;
            AudioRecordingRequired = audioRecordingRequired;
        }

        public bool CanAllocate(JusticeUser user, AllocateHearingConfiguration configuration)
        {
            if (ScheduledDateTime.Date != ScheduledEndTime.Date)
            {
                throw new DomainRuleException("AllocationNotSupported",
                    $"Unable to allocate to hearing {Id}, hearings which span multiple days are not currently supported");
            }

            if (!user.IsAvailable(ScheduledDateTime, ScheduledEndTime, configuration))
            {
                return false;
            }

            var allocations = user.Allocations
                // Only those that end on or after this hearing's start time, plus our minimum allowed gap
                .Where(a => a.Hearing.ScheduledDateTime.AddMinutes(a.Hearing.ScheduledDuration + configuration.MinimumGapBetweenHearingsInMinutes) >= ScheduledDateTime)
                .ToList();

            var gapBetweenHearingsIsInsufficient = allocations.Any(a => (ScheduledDateTime - a.Hearing.ScheduledDateTime).TotalMinutes < configuration.MinimumGapBetweenHearingsInMinutes);
            if (gapBetweenHearingsIsInsufficient)
            {
                return false;
            }
            
            var concurrentAllocatedHearings = CountConcurrentAllocatedHearings(allocations);
            if (concurrentAllocatedHearings > configuration.MaximumConcurrentHearings)
            {
                return false;
            }

            return true;
        }
        
        private int CountConcurrentAllocatedHearings(IEnumerable<Allocation> allocations)
        {
            var hearingsToCheck = allocations
                .Select(a => new
                {
                    StartDate = a.Hearing.ScheduledDateTime,
                    EndDate = a.Hearing.ScheduledEndTime
                })
                .OrderBy(a => a.StartDate)
                .ToList();
    
            hearingsToCheck.Add(new
            {
                StartDate = ScheduledDateTime, 
                EndDate = ScheduledEndTime
            });
            
            var minEndTime = hearingsToCheck.Min(a => a.EndDate);
            var count = hearingsToCheck.Count(a => a.StartDate < minEndTime);

            return count;
        }

        private bool DoesParticipantExistByContactEmail(string contactEmail)
        {
            return Participants.Any(x => x.Person.ContactEmail == contactEmail);
        }

        private bool DoesParticipantExistByUsername(string username)
        {
            return Participants.Any(x => x.Person.Username == username);
        }

        private bool DoesEndpointExist(string sip)
        {
            return Endpoints.Any(x => x.Sip.Equals(sip, StringComparison.InvariantCultureIgnoreCase));
        }
        
        private void ValidateArguments(DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue,
            HearingType hearingType)
        {
            ValidateScheduledDate(scheduledDateTime);

            if (scheduledDuration <= 0)
            {
                _validationFailures.AddFailure("ScheduledDuration", "ScheduledDuration is not a valid value");
            }

            if (hearingVenue == null || hearingVenue.Id <= 0)
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
            if (scheduledDateTime == default)
            {
                _validationFailures.AddFailure(nameof(ScheduledDateTime), "ScheduledDateTime is not a valid value");
            }
            if (scheduledDateTime.Date.CompareTo(DateTime.UtcNow.Date) < 0)
            {
                _validationFailures.AddFailure(nameof(ScheduledDateTime), "Schedule datetime cannot be set in the past");
            }
        }

        public void UpdateStatus(BookingStatus newStatus, string updatedBy, string cancelReason)
        {
            if (string.IsNullOrEmpty(updatedBy))
            {
                throw new ArgumentNullException(nameof(updatedBy));
            }

            if (newStatus == BookingStatus.Cancelled && string.IsNullOrEmpty(cancelReason))
            {
                throw new ArgumentNullException(nameof(cancelReason));
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
            CancelReason = cancelReason;
            if(newStatus == BookingStatus.Created)
            {
                // Booking confirmed
                ConfirmedBy = updatedBy;
                ConfirmedDate = DateTime.UtcNow;
            }

            if (newStatus == BookingStatus.Cancelled)
            {
                Deallocate();
            }
        }

        public void Deallocate()
        {
            Allocations?.Clear();
        }
    }
}