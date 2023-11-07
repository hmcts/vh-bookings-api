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
        private readonly ValidationFailures _validationFailures = new ();
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
            JudiciaryParticipants = new List<JudiciaryParticipant>();
        }

        /// <summary>
        /// Instantiate a hearing when the hearing type is known, typically used for V1
        /// </summary>
        protected Hearing(
            CaseType caseType, 
            HearingType hearingType,
            DateTime scheduledDateTime,
            int scheduledDuration, 
            HearingVenue hearingVenue, 
            string hearingRoomName,
            string otherInformation, 
            string createdBy, 
            bool audioRecordingRequired, 
            string cancelReason)
            : this()
        {
            ValidateArguments(scheduledDateTime, scheduledDuration, hearingVenue);

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            CaseTypeId = caseType.Id;
            HearingTypeId = hearingType?.Id;
            HearingVenueId = hearingVenue.Id;

            Status = BookingStatus.Booked;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            CreatedBy = createdBy;
            AudioRecordingRequired = audioRecordingRequired;
            CancelReason = cancelReason;
        }

        /// <summary>
        /// Instantiate a hearing without a hearing type, typically used for V2
        /// </summary>
        protected Hearing(
            CaseType caseType,
            DateTime scheduledDateTime,
            int scheduledDuration,
            HearingVenue hearingVenue,
            string hearingRoomName,
            string otherInformation,
            string createdBy,
            bool audioRecordingRequired,
            string cancelReason) : this(caseType, null, scheduledDateTime, scheduledDuration, hearingVenue,
            hearingRoomName, otherInformation, createdBy, audioRecordingRequired, cancelReason)
        {}

        public abstract HearingMediumType HearingMediumType { get; protected set; }
        public virtual HearingVenue HearingVenue { get; protected set; }
        public int? HearingVenueId { get; set; }
        public int CaseTypeId { get; set; }
        public virtual CaseType CaseType { get; set; }
        public int? HearingTypeId { get; set; }
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
        public virtual IList<JudiciaryParticipant> JudiciaryParticipants { get; }
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }
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
                throw new DomainRuleException("Case", $"Case {number} - {name} already exists for the hearing");
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
                
                throw new DomainRuleException(nameof(endpoint), $"Endpoint {endpoint.Sip} already exists in the hearing");
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
            if (hearingRole.IsInterpreter() && IsHearingConfirmedAndCloseToStartTime())
            {
                throw new DomainRuleException("Hearing", DomainRuleErrorMessages.CannotAddInterpreterToHearingCloseToStartTime);
            }
            if (DoesParticipantExistByContactEmail(person.ContactEmail))
            {
                throw new DomainRuleException(nameof(person), $"Participant {person.ContactEmail} already exists in the hearing");
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
            if(!hearingRole.IsJudge())
            {
                throw new DomainRuleException(nameof(hearingRole), "Hearing role should be Judge");
            }

            if (DoesParticipantExistByUsername(person.Username))
            {
                throw new DomainRuleException(nameof(person), "Judge with given username already exists in the hearing");
            }

            if (DoesJudgeExist())
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

        public void RemoveJudiciaryParticipantByPersonalCode(string judiciaryParticipantPersonalCode)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotRemoveJudiciaryParticipantCloseToStartTime);
            if (!DoesJudiciaryParticipantExistByPersonalCode(judiciaryParticipantPersonalCode))
            {
                throw new DomainRuleException(nameof(judiciaryParticipantPersonalCode),
                    DomainRuleErrorMessages.JudiciaryParticipantNotFound);
            }

            var existingParticipant = JudiciaryParticipants.Single(x =>
                x.JudiciaryPerson.PersonalCode == judiciaryParticipantPersonalCode);
            JudiciaryParticipants.Remove(existingParticipant);
            ValidateHostCount();
            UpdateBookingStatusJudgeRequirement();
            UpdatedDate = DateTime.UtcNow;
        }

        public bool HasHost =>
            GetParticipants().Any(x => x.HearingRole.Name == "Judge" || x.HearingRole.Name == "Staff Member") ||
            JudiciaryParticipants.Any(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);

        public JudiciaryParticipant AddJudiciaryJudge(JudiciaryPerson judiciaryPerson, string displayName)
        {
            ValidateAddJudiciaryParticipant(judiciaryPerson);
            
            if (DoesJudgeExist())
            {
                throw new DomainRuleException(nameof(judiciaryPerson), DomainRuleErrorMessages.ParticipantWithJudgeRoleAlreadyExists);
            }

            var participant = new JudiciaryParticipant(displayName, judiciaryPerson, JudiciaryParticipantHearingRoleCode.Judge);
            JudiciaryParticipants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            UpdateBookingStatusJudgeRequirement();
            return participant;
        }
        
        public JudiciaryParticipant AddJudiciaryPanelMember(JudiciaryPerson judiciaryPerson, string displayName)
        {
            ValidateAddJudiciaryParticipant(judiciaryPerson);
            
            var participant = new JudiciaryParticipant(displayName, judiciaryPerson, JudiciaryParticipantHearingRoleCode.PanelMember);
            JudiciaryParticipants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            
            return participant;
        }

        public JudiciaryParticipant UpdateJudiciaryParticipantByPersonalCode(string personalCode, string newDisplayName, 
            JudiciaryParticipantHearingRoleCode newHearingRoleCode)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotUpdateJudiciaryParticipantCloseToStartTime);
            if (!DoesJudiciaryParticipantExistByPersonalCode(personalCode))
            {
                throw new DomainRuleException(nameof(personalCode), DomainRuleErrorMessages.JudiciaryParticipantNotFound);
            }
            
            if (newHearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge && DoesJudgeExist(personalCodeToIgnore: personalCode))
            {
                throw new DomainRuleException(nameof(personalCode), DomainRuleErrorMessages.ParticipantWithJudgeRoleAlreadyExists);
            }
            
            var participant = JudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCode);
            if (participant == null)
            {
                throw new InvalidOperationException($"{nameof(participant)} cannot be null");
            }
            
            participant.UpdateDisplayName(newDisplayName);
            participant.UpdateHearingRoleCode(newHearingRoleCode);
            ValidateHostCount();
            UpdatedDate = DateTime.UtcNow;
            
            
            return participant;
        }

        private void ValidateAddJudiciaryParticipant(JudiciaryPerson judiciaryPerson)
        {
            if (DoesJudiciaryParticipantExistByPersonalCode(judiciaryPerson.PersonalCode))
            {
                throw new DomainRuleException(nameof(judiciaryPerson), "Judiciary participant already exists in the hearing");
            }

            if (judiciaryPerson.IsALeaver())
            {
                throw new DomainRuleException(nameof(judiciaryPerson), "Cannot add a participant who is a leaver");
            }
        }

        public void ValidateHostCount()
        {
            if (!HasHost && Status is BookingStatus.Booked or BookingStatus.Created)
            {
                throw new DomainRuleException("Host", DomainRuleErrorMessages.HearingNeedsAHost);
            }
        }

        public void RemoveParticipant(Participant participant, bool validateParticipantCount=true)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotRemoveParticipantCloseToStartTime);
            if (!DoesParticipantExistByContactEmail(participant.Person.ContactEmail))
            {
                throw new DomainRuleException("Participant", "Participant does not exist on the hearing");
            }

            if (validateParticipantCount) ValidateHostCount();

            var existingParticipant = Participants.Single(x => x.Person.ContactEmail == participant.Person.ContactEmail);
            var endpoint = Endpoints.SingleOrDefault(e => e.DefenceAdvocate != null && e.DefenceAdvocate.Id == participant.Id);
            endpoint?.AssignDefenceAdvocate(null);

            participant.LinkedParticipants.Clear();

            Participants.Remove(existingParticipant);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveParticipantById(Guid participantId, bool validateParticipantCount=true)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotRemoveParticipantCloseToStartTime);
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
        
        public IList<JudiciaryParticipant> GetJudiciaryParticipants()
        {
            return JudiciaryParticipants;
        }

        public void RemoveEndpoint(Endpoint endpoint)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotRemoveAnEndpointCloseToStartTime);
            endpoint.AssignDefenceAdvocate(null);
            Endpoints.Remove(endpoint);
            UpdatedDate = DateTime.UtcNow;
        }

        public virtual void UpdateCase(Case @case)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotUpdateACaseCloseToStartTime);
            //It has been assumed that only one case exists for a given hearing, for now.
            var existingCase = GetCases().FirstOrDefault();
            if (existingCase == null) return;
            existingCase.Number = @case.Number;
            existingCase.Name = @case.Name;
        }

        public void UpdateHearingDetails(HearingVenue hearingVenue, DateTime scheduledDateTime,
            int scheduledDuration, string hearingRoomName, string otherInformation, string updatedBy,
            List<Case> cases, bool audioRecordingRequired)
        {
            ValidateScheduledDate(scheduledDateTime);

            if (scheduledDuration <= 0)
            {
                _validationFailures.AddFailure("ScheduledDuration", "ScheduledDuration is not a valid value");
            }

            if (hearingVenue is not {Id: > 0})
            {
                _validationFailures.AddFailure("Venue", "Venue must have a valid value");
            }

            if (_validationFailures.Any())
            {
                throw new DomainRuleException(_validationFailures);
            }
            
            // all the properties are the same, so no need to update
            if (hearingVenue.VenueCode == HearingVenue.VenueCode && scheduledDateTime == ScheduledDateTime &&
                scheduledDuration == ScheduledDuration && hearingRoomName == HearingRoomName &&
                otherInformation == OtherInformation && audioRecordingRequired == AudioRecordingRequired)
            {
                return;
            }

            ValidateChangeAllowed(DomainRuleErrorMessages.CannotUpdateHearingDetailsCloseToStartTime);
            

            HearingVenue = hearingVenue;

            if (cases.Any())
            {
                UpdateCase(cases[0]);
            }

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;

            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.UtcNow;
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

            var gapBetweenHearingsIsInsufficient = allocations.Exists(a =>
            {
                var timeDifferenceInMinutes = Math.Abs((ScheduledDateTime - a.Hearing.ScheduledDateTime).TotalMinutes);

                return timeDifferenceInMinutes < configuration.MinimumGapBetweenHearingsInMinutes;
            });
            
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

        public bool IsJusticeUserAllocated()
        {
            return Allocations.Any();
        }
        
        public void AllocateJusticeUser(JusticeUser user)
        {
            if (Allocations.Any(x => x.JusticeUserId == user.Id))
            {
                throw new DomainRuleException("Allocation", $"User {user.Id} is already allocated to hearing {Id}");

            }

            Allocations.Add(new Allocation
            {
                Hearing = this,
                JusticeUserId = user.Id,
            });
        }

        public void Deallocate()
        {
            Allocations?.Clear();
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

        private bool DoesJudiciaryParticipantExistByPersonalCode(string personalCode)
        {
            return JudiciaryParticipants.Any(x => x.JudiciaryPerson.PersonalCode == personalCode);
        }

        private bool DoesJudgeExist(string personalCodeToIgnore = null)
        {
            var judiciaryJudges = JudiciaryParticipants.Where(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            if (!string.IsNullOrEmpty(personalCodeToIgnore))
            {
                judiciaryJudges = judiciaryJudges.Where(x => x.JudiciaryPerson.PersonalCode != personalCodeToIgnore);
            }
            
            return Participants.Any(x => x is Judge) || judiciaryJudges.Any();
        }

        private void ValidateArguments(DateTime scheduledDateTime, int scheduledDuration, HearingVenue hearingVenue)
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
        
        /// <summary>
        /// Validates if the hearing can be changed if it meets the criteria:
        /// <list type="bullet">
        ///     <item>Not cancelled</item>
        ///     <item>Booked Or Failed</item>
        ///     <item>Created and not scheduled to start in 30 minutes</item>
        /// </list>
        /// </summary>
        /// <exception cref="DomainRuleException">Offending validation rule</exception>
        public void ValidateChangeAllowed(string errorMessage = null)
        {
            if(Status == BookingStatus.Cancelled)
            {
                throw new DomainRuleException("Hearing", errorMessage ?? DomainRuleErrorMessages.CannotEditACancelledHearing);
            }
            
            if (Status == BookingStatus.Created && IsHearingConfirmedAndCloseToStartTime())
            {
                throw new DomainRuleException("Hearing", errorMessage ?? DomainRuleErrorMessages.DefaultCannotEditAHearingCloseToStartTime);
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

        public ParticipantBase GetJudge()
        {
            var judge = Participants.FirstOrDefault(p => p is Judge);
            var judiciaryJudge = JudiciaryParticipants?.FirstOrDefault(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);

            return (ParticipantBase)judge ?? judiciaryJudge;
        }

        public void UpdateBookingStatusJudgeRequirement()
        {
            if (GetJudge() == null)
                Status = Status switch
                {
                    BookingStatus.Booked => BookingStatus.BookedWithoutJudge,
                    BookingStatus.Created => BookingStatus.ConfirmedWithoutJudge,
                    _ => Status
                };
            else
                Status = Status switch
                {
                    BookingStatus.BookedWithoutJudge => BookingStatus.Booked,
                    BookingStatus.ConfirmedWithoutJudge => BookingStatus.Created,
                    _ => Status 
                };
        }

        private bool IsHearingConfirmedAndCloseToStartTime() => ScheduledDateTime.AddMinutes(-30) <= DateTime.UtcNow &&
                                                                (Status == BookingStatus.Created ||
                                                                 Status == BookingStatus.ConfirmedWithoutJudge);

    }
}