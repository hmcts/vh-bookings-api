using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.Ddd;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.JudiciaryParticipants;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public abstract class Hearing : TrackableAggregateRoot<Guid>
    {
        private readonly ValidationFailures _validationFailures = new();
        private bool _isFirstDayOfMultiDayHearing;

        protected Hearing()
        {
            Id = Guid.NewGuid();
            Cases = new List<Case>();
            Participants = new List<Participant>();
            HearingCases = new List<HearingCase>();
            Endpoints = new List<Endpoint>();
            Allocations = new List<Allocation>();
            JudiciaryParticipants = new List<JudiciaryParticipant>();
            ConferenceSupplier = VideoSupplier.Vodafone;
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
            bool audioRecordingRequired) : this()
        {
            ValidateArguments(scheduledDateTime, scheduledDuration, hearingVenue);
            
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            CaseTypeId = caseType.Id;
            HearingVenueId = hearingVenue.Id;
            
            Status = BookingStatus.Booked;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            CreatedBy = createdBy;
            AudioRecordingRequired = audioRecordingRequired;
        }

        public abstract HearingMediumType HearingMediumType { get; protected set; }
        public virtual HearingVenue HearingVenue { get; protected set; }
        public int? HearingVenueId { get; set; }
        public int CaseTypeId { get; set; }
        public virtual CaseType CaseType { get; set; }
        protected virtual IList<Case> Cases { get; set; }
        public DateTime ScheduledDateTime { get; protected set; }
        public int ScheduledDuration { get; protected set; }
        public BookingStatus Status { get; protected set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTime? ConfirmedDate { get; protected set; }
        public IList<Participant> Participants { get; }
        public IList<Endpoint> Endpoints { get; }
        public IList<HearingCase> HearingCases { get; set; }
        public IList<JudiciaryParticipant> JudiciaryParticipants { get; }
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }
        public bool AudioRecordingRequired { get; set; }
        public string CancelReason { get; set; }
        public Guid? SourceId { get; set; }
        public VideoSupplier ConferenceSupplier { get; protected set; }

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
                throw new DomainRuleException(nameof(endpoint),
                    $"Endpoint {endpoint.Sip} already exists in the hearing");
            }

            Participant defenceAdvocate = null;

            if (endpoint.DefenceAdvocate != null)
            {
                defenceAdvocate = Participants.Single(x => x.Id == endpoint.DefenceAdvocate.Id);
            }

            var newEndpoint = new Endpoint(endpoint.ExternalReferenceId, endpoint.DisplayName, endpoint.Sip, endpoint.Pin, defenceAdvocate);
            newEndpoint.UpdateExternalIds(endpoint.ExternalReferenceId, endpoint.MeasuresExternalId);
            newEndpoint.UpdateLanguagePreferences(endpoint.InterpreterLanguage, endpoint.OtherLanguage);
            Endpoints.Add(newEndpoint);
            UpdatedDate = DateTime.UtcNow;
        }

        public void AddEndpoints(List<Endpoint> endpoints)
        {
            endpoints.ForEach(AddEndpoint);
        }
        
        public Participant AddIndividual(string externalReferenceId, Person person, HearingRole hearingRole, string displayName)
        {
            ValidateParticipantDoesNotExistInHearing(externalReferenceId, person);

            var participant = new Individual(externalReferenceId, person, hearingRole, displayName)
            {
                CreatedBy = CreatedBy
            };
            Participants.Add(participant);

            if (hearingRole.IsInterpreter() && CaseType.IsAudioRecordingAllowed)
            {
                AudioRecordingRequired = true;
            }

            UpdatedDate = DateTime.UtcNow;

            return participant;
        }
        
        public Participant AddRepresentative(string externalReferenceId, Person person, HearingRole hearingRole, string displayName, string representee)
        {
            ValidateParticipantDoesNotExistInHearing(externalReferenceId, person);

            var participant = new Representative(externalReferenceId, person, hearingRole, displayName, representee)
            {
                CreatedBy = CreatedBy
            };
            
            Participants.Add(participant);
            UpdatedDate = DateTime.UtcNow;

            return participant;
        }

        /// <summary>
        /// Check if a participant with the given external reference id already exists in the hearing or by contact email
        /// </summary>
        private void ValidateParticipantDoesNotExistInHearing(string externalReferenceId, Person person)
        {
            if (externalReferenceId != null && Participants.Any(x=> x.ExternalReferenceId == externalReferenceId))
            {
                throw new DomainRuleException(nameof(Participant),
                    $"Participant with external reference id {externalReferenceId} already exists in the hearing");
            }
            
            if (person.ContactEmail != null && DoesParticipantExistByContactEmail(person.ContactEmail))
            {
                throw new DomainRuleException(nameof(person),
                    $"Participant {person.ContactEmail} already exists in the hearing");
            }
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
            UpdateBookingStatusJudgeRequirement();
            UpdatedDate = DateTime.UtcNow;
        }

        public bool HasHost =>
            JudiciaryParticipants.Any(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);

        public JudiciaryParticipant AddJudiciaryJudge(JudiciaryPerson judiciaryPerson, string displayName,
            string email = null, string phone = null, InterpreterLanguage interpreterLanguage = null,
            string otherLanguage = null)
        {
            ValidateAddJudiciaryParticipant(judiciaryPerson);

            if (DoesJudgeExist())
                throw new DomainRuleException(nameof(judiciaryPerson),
                    DomainRuleErrorMessages.ParticipantWithJudgeRoleAlreadyExists);

            var participant = new JudiciaryParticipant(displayName, judiciaryPerson,
                JudiciaryParticipantHearingRoleCode.Judge, email, phone);
            participant.UpdateLanguagePreferences(interpreterLanguage, otherLanguage);

            JudiciaryParticipants.Add(participant);
            UpdatedDate = DateTime.UtcNow;
            UpdateBookingStatusJudgeRequirement();
            return participant;
        }

        public JudiciaryParticipant AddJudiciaryPanelMember(JudiciaryPerson judiciaryPerson, string displayName,
            string email = null, string phone = null, InterpreterLanguage interpreterLanguage = null,
            string otherLanguage = null)
        {
            ValidateAddJudiciaryParticipant(judiciaryPerson);
            if (DoesJudiciaryParticipantExistByPersonalCode(judiciaryPerson.PersonalCode))
            {
                throw new DomainRuleException(nameof(judiciaryPerson),
                    $"Judiciary Person {judiciaryPerson.PersonalCode} already exists in the hearing");
            }

            var participant = new JudiciaryParticipant(displayName,
                judiciaryPerson,
                JudiciaryParticipantHearingRoleCode.PanelMember,
                email, phone);
            participant.UpdateLanguagePreferences(interpreterLanguage, otherLanguage);
            JudiciaryParticipants.Add(participant);
            UpdatedDate = DateTime.UtcNow;

            return participant;
        }

        public JudiciaryParticipant UpdateJudiciaryParticipantByPersonalCode(string personalCode, string newDisplayName,
            JudiciaryParticipantHearingRoleCode newHearingRoleCode, InterpreterLanguage interpreterLanguage,
            string otherLanguage)
        {
            if (!DoesJudiciaryParticipantExistByPersonalCode(personalCode))
            {
                throw new DomainRuleException(nameof(personalCode),
                    DomainRuleErrorMessages.JudiciaryParticipantNotFound);
            }

            if (newHearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge &&
                DoesJudgeExist(personalCodeToIgnore: personalCode))
            {
                throw new DomainRuleException(nameof(personalCode),
                    DomainRuleErrorMessages.ParticipantWithJudgeRoleAlreadyExists);
            }

            var participant = JudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCode);
            if (participant == null)
            {
                throw new InvalidOperationException($"{nameof(participant)} cannot be null");
            }

            var hasChanged = newDisplayName != participant.DisplayName ||
                             newHearingRoleCode != participant.HearingRoleCode ||
                             interpreterLanguage?.Code != participant.InterpreterLanguage?.Code ||
                             otherLanguage != participant.OtherLanguage;
            if (!hasChanged)
                return participant;

            ValidateChangeAllowed(DomainRuleErrorMessages.CannotUpdateJudiciaryParticipantCloseToStartTime);

            participant.UpdateDisplayName(newDisplayName);
            participant.UpdateHearingRoleCode(newHearingRoleCode);
            participant.UpdateLanguagePreferences(interpreterLanguage, otherLanguage);
            ValidateHostCount();
            UpdatedDate = DateTime.UtcNow;


            return participant;
        }

        private void ValidateAddJudiciaryParticipant(JudiciaryPerson judiciaryPerson, string roleToIgnore = null)
        {
            if (DoesJudiciaryParticipantExistByPersonalCode(judiciaryPerson.PersonalCode, roleToIgnore: roleToIgnore))
            {
                throw new DomainRuleException(nameof(judiciaryPerson),
                    DomainRuleErrorMessages.JudiciaryPersonAlreadyExists(judiciaryPerson.PersonalCode));
            }

            if (judiciaryPerson.IsALeaver())
            {
                throw new DomainRuleException(nameof(judiciaryPerson),
                    DomainRuleErrorMessages.CannotAddLeaverJudiciaryPerson);
            }

            if (judiciaryPerson.Deleted)
            {
                throw new DomainRuleException(nameof(judiciaryPerson),
                    DomainRuleErrorMessages.CannotAddDeletedJudiciaryPerson);
            }
        }

        public void ValidateHostCount()
        {
            if (!HasHost && Status is BookingStatus.Booked or BookingStatus.Created)
            {
                throw new DomainRuleException("Host", DomainRuleErrorMessages.HearingNeedsAHost);
            }
        }

        public void RemoveParticipant(Participant participant, bool validateParticipantCount = true)
        {
            ValidateChangeAllowed(DomainRuleErrorMessages.CannotRemoveParticipantCloseToStartTime);
            if (!DoesParticipantExistByContactEmail(participant.Person.ContactEmail))
            {
                throw new DomainRuleException("Participant", DomainRuleErrorMessages.ParticipantDoesNotExist);
            }

            if (validateParticipantCount) ValidateHostCount();

            var existingParticipant =
                Participants.Single(x => x.Person.ContactEmail == participant.Person.ContactEmail);
            var endpoint =
                Endpoints.SingleOrDefault(e => e.DefenceAdvocate != null && e.DefenceAdvocate.Id == participant.Id);
            endpoint?.AssignDefenceAdvocate(null);

            participant.LinkedParticipants.Clear();

            // update screening list
            Participants
                .Where(existingPat => existingPat.Screening != null)
                .ToList()
                .ForEach(existingPat => existingPat.Screening.RemoveParticipant(participant));

            Endpoints.Where(existingEndpoint => existingEndpoint.Screening != null)
                .ToList()
                .ForEach(existingEndpoint => existingEndpoint.Screening.RemoveParticipant(participant));

            Participants.Remove(existingParticipant);
            UpdatedDate = DateTime.UtcNow;
        }

        public void RemoveParticipantById(Guid participantId, bool validateParticipantCount = true)
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

            Participants
                .Where(existingPat => existingPat.Screening != null)
                .ToList()
                .ForEach(existingPat => existingPat.Screening.RemoveEndpoint(endpoint));

            Endpoints.Where(existingEndpoint => existingEndpoint.Screening != null)
                .ToList()
                .ForEach(existingEndpoint => existingEndpoint.Screening.RemoveEndpoint(endpoint));

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

        public void RenameHearingForMultiDayBooking(string newCaseName)
        {
            if (SourceId == null)
            {
                throw new DomainRuleException("CaseName", DomainRuleErrorMessages.HearingNotMultiDay);
            }

            var existingCase = GetCases().FirstOrDefault();
            if (existingCase == null) return;
            existingCase.Name = newCaseName;
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

            if (hearingVenue is not { Id: > 0 })
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
                otherInformation == OtherInformation && audioRecordingRequired == AudioRecordingRequired &&
                !CasesHaveChanged(cases))
            {
                // Need to update these details for Admin Web
                UpdateHearingUpdatedAuditDetails(updatedBy);

                return;
            }

            ValidateChangeAllowed(DomainRuleErrorMessages.CannotUpdateHearingDetailsCloseToStartTime);


            HearingVenue = hearingVenue;

            if (cases.Count > 0)
            {
                UpdateCase(cases[0]);
            }

            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;

            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdateHearingUpdatedAuditDetails(updatedBy);
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
                .Where(a => a.Hearing.ScheduledDateTime.AddMinutes(a.Hearing.ScheduledDuration +
                                                                   configuration.MinimumGapBetweenHearingsInMinutes) >=
                            ScheduledDateTime)
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

        private bool DoesEndpointExist(string sip)
        {
            return Endpoints.Any(x => x.Sip.Equals(sip, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool DoesJudiciaryParticipantExistByPersonalCode(string personalCode, string roleToIgnore = null)
        {
            var judiciaryParticipants =
                JudiciaryParticipants.Where(x => x.JudiciaryPerson.PersonalCode == personalCode);
            if (!string.IsNullOrEmpty(roleToIgnore))
            {
                judiciaryParticipants = judiciaryParticipants.Where(x => x.HearingRoleCode.ToString() != roleToIgnore);
            }

            return judiciaryParticipants.Any();
        }

        private bool DoesJudgeExist(string personalCodeToIgnore = null)
        {
            var judiciaryJudges =
                JudiciaryParticipants.Where(x => x.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            if (!string.IsNullOrEmpty(personalCodeToIgnore))
            {
                judiciaryJudges = judiciaryJudges.Where(x => x.JudiciaryPerson.PersonalCode != personalCodeToIgnore);
            }

            return judiciaryJudges.Any();
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
                _validationFailures.AddFailure(nameof(ScheduledDateTime),
                    "Schedule datetime cannot be set in the past");
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
            if (Status == BookingStatus.Cancelled)
            {
                throw new DomainRuleException(nameof(Hearing),
                    errorMessage ?? DomainRuleErrorMessages.CannotEditACancelledHearing);
            }

            if (Status is BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge &&
                IsHearingConfirmedAndCloseToStartTime())
            {
                throw new DomainRuleException(nameof(Hearing),
                    errorMessage ?? DomainRuleErrorMessages.DefaultCannotEditAHearingCloseToStartTime);
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

            newStatus = newStatus == BookingStatus.Created && GetJudge() == null
                ? BookingStatus.ConfirmedWithoutJudge
                : newStatus;

            var bookingStatusTransition = new BookingStatusTransition();
            var statusChangedEvent = new StatusChangedEvent(Status, newStatus);

            if (!bookingStatusTransition.IsValid(statusChangedEvent))
            {
                throw new DomainRuleException("BookingStatus",
                    $"Cannot change the booking status from {Status} to {newStatus}");
            }

            Status = newStatus;
            UpdatedDate = DateTime.UtcNow;
            UpdatedBy = updatedBy;
            CancelReason = cancelReason;
            if (newStatus == BookingStatus.Created || newStatus == BookingStatus.ConfirmedWithoutJudge)
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

        /// <summary>
        /// ### important! Ensure that this hearing, participant and judiciaryParticipant entities are eagerly loaded before invoking this method.
        /// Will assume there is no judge the properties are null
        /// </summary>
        public JudiciaryParticipant GetJudge()
        {
            return GetJudiciaryJudge();
        }

        private JudiciaryParticipant GetJudiciaryJudge()
        {
            var judiciaryJudge =
                JudiciaryParticipants?.FirstOrDefault(p =>
                    p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);

            return judiciaryJudge;
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

        /// <summary>
        /// Assign the screening (special measure) rules for a participant
        /// </summary>
        /// <param name="participant">The participant who needs screening</param>
        /// <param name="screeningType">The type of screening, blanket (all) or specific participants</param>
        /// <param name="protectedFrom">The list of external reference ids of entities to screen from</param>
        public void AssignScreeningForParticipant(Participant participant, ScreeningType screeningType, List<string> protectedFrom)
        {
            ArgumentNullException.ThrowIfNull(protectedFrom);
            // check if participantContactEmail contains the participant's contact email
            if (participant.ExternalReferenceId != null && protectedFrom.Contains(participant.ExternalReferenceId))
            {
                throw new DomainRuleException("Participant", DomainRuleErrorMessages.ParticipantCannotScreenFromThemself);
            }

            var matched = GetMatchingParticipantsAndEndpointsByExternalReferenceIds(protectedFrom);
            var originalParticipantUpdatedDate = participant.UpdatedDate;
            
            participant.AssignScreening(screeningType, matched.participants, matched.endpoints);

            var hasChanged = participant.UpdatedDate != originalParticipantUpdatedDate;
            if (!hasChanged) return;
            
            UpdatedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Assign the screening (special measure) rules for an endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint who needs screening</param>
        /// <param name="screeningType">The type of screening, blanket (all) or specific participants</param>
        /// <param name="protectedFrom">The list of external reference ids of entities to screen from</param>
        public void AssignScreeningForEndpoint(Endpoint endpoint, ScreeningType screeningType, List<string> protectedFrom)
        {
            ArgumentNullException.ThrowIfNull(protectedFrom);
            if(endpoint.ExternalReferenceId != null && protectedFrom.Contains(endpoint.ExternalReferenceId))
            {
                throw new DomainRuleException("Endpoint", DomainRuleErrorMessages.EndpointCannotScreenFromThemself);
            }
            
            var matched = GetMatchingParticipantsAndEndpointsByExternalReferenceIds(protectedFrom);
            
            endpoint.AssignScreening(screeningType, matched.participants, matched.endpoints);
            UpdatedDate = DateTime.UtcNow;
        }

        public void OverrideSupplier(VideoSupplier commandVideoSupplier)
        {
            if (Status is BookingStatus.ConfirmedWithoutJudge or BookingStatus.Created)
            {
                throw new DomainRuleException(nameof(ConferenceSupplier),
                    DomainRuleErrorMessages.ConferenceSupplierAlreadyExists);
            }

            ConferenceSupplier = commandVideoSupplier;
        }
        
        public void ReassignJudiciaryJudge(JudiciaryJudge newJudge, InterpreterLanguage interpreterLanguage = null,
            string otherLanguage = null)
        {
            ArgumentNullException.ThrowIfNull(newJudge);

            ValidateReassignJudgeAllowed();
            ValidateAddJudiciaryParticipant(newJudge.JudiciaryPerson,
                roleToIgnore: newJudge.HearingRoleCode.ToString());

            var existingJudge =
                JudiciaryParticipants.FirstOrDefault(
                    p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            if (existingJudge != null)
            {
                JudiciaryParticipants.Remove(existingJudge);
            }

            newJudge.UpdateLanguagePreferences(interpreterLanguage, otherLanguage);
            JudiciaryParticipants.Add(newJudge);

            UpdatedDate = DateTime.UtcNow;
            UpdateBookingStatusJudgeRequirement();
        }

        private void ValidateReassignJudgeAllowed()
        {
            if (Status == BookingStatus.Cancelled)
            {
                throw new DomainRuleException(nameof(Hearing), DomainRuleErrorMessages.CannotEditACancelledHearing);
            }
        }

        private bool IsHearingConfirmedAndCloseToStartTime() => ScheduledDateTime.AddMinutes(-30) <= DateTime.UtcNow &&
                                                                (Status == BookingStatus.Created ||
                                                                 Status == BookingStatus.ConfirmedWithoutJudge);

        private void UpdateHearingUpdatedAuditDetails(string updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.UtcNow;
        }

        private bool CasesHaveChanged(IReadOnlyList<Case> cases)
        {
            var existingCase = GetCases().FirstOrDefault();

            if (!cases.Any() || existingCase == null)
            {
                return false;
            }

            return cases[0].Number != existingCase.Number || cases[0].Name != existingCase.Name;
        }
        
        private (List<Participant> participants, List<Endpoint> endpoints) GetMatchingParticipantsAndEndpointsByExternalReferenceIds(List<string> protectedFrom)
        {
            var participants = new List<Participant>();
            var endpoints = new List<Endpoint>();
            var notFoundList = new List<string>();
            
            foreach (var entry in protectedFrom)
            {
                var matchedParticipant = Participants.FirstOrDefault(p => p.ExternalReferenceId == entry);
                var matchedEndpoint = Endpoints.FirstOrDefault(e => e.ExternalReferenceId == entry);

                if (matchedParticipant != null)
                {
                    participants.Add(matchedParticipant);
                }
                else if (matchedEndpoint != null)
                {
                    endpoints.Add(matchedEndpoint);
                }
                else
                {
                    notFoundList.Add(entry);
                }
            }

            if (notFoundList.Count > 0)
            {
                throw new DomainRuleException("ProtectedFrom", $"The following external IDs were not found in the hearing: {string.Join(", ", notFoundList)}");
            }
            
            return (participants, endpoints);
        }
    }
}