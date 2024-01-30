using BookingsApi.Contract.V1.Requests;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        List<ExistingParticipantDetails> ExtractExistingParticipants(VideoHearing hearing, List<EditableParticipantRequest> participants);
        List<NewParticipant> ExtractNewParticipants(VideoHearing hearing, List<EditableParticipantRequest> participants, bool ejudFeatureEnabled);
        List<Guid> ExtractRemovedParticipantIds(VideoHearing hearing, List<EditableParticipantRequest> participants);
        List<LinkedParticipantDto> ExtractLinkedParticipants(VideoHearing hearing, 
            List<EditableParticipantRequest> participants, 
            List<ExistingParticipantDetails> existingParticipants,
            List<NewParticipant> newParticipants);
        void AssignParticipantIdsForEditMultiDayHearingFutureDay(VideoHearing multiDayHearingFutureDay,
            List<EditableParticipantRequest> participants);
    }
    
    public class UpdateHearingService : IUpdateHearingService
    {
        public List<ExistingParticipantDetails> ExtractExistingParticipants(VideoHearing hearing, List<EditableParticipantRequest> participants)
        {
            var existingParticipants = new List<ExistingParticipantDetails>();
            
            foreach (var participant in participants)
            {
                if (participant.Id == null)
                    // Participant is new, not existing
                    continue;
                
                var existingParticipant = hearing.Participants.FirstOrDefault(p => p.Id.Equals(participant.Id));
                if (existingParticipant == null)
                    continue;
                
                // Note - copied from the UpdateHearingParticipants endpoint, TODO refactor
                var existingParticipantDetail = new ExistingParticipantDetails
                {
                    DisplayName = participant.DisplayName,
                    OrganisationName = participant.OrganisationName,
                    ParticipantId = participant.Id.Value,
                    Person = existingParticipant.Person,
                    RepresentativeInformation = new RepresentativeInformation { Representee = participant.Representee },
                    TelephoneNumber = participant.TelephoneNumber,
                    Title = participant.Title
                };
                existingParticipantDetail.Person.ContactEmail = participant.ContactEmail ?? existingParticipant.Person.ContactEmail;
                existingParticipants.Add(existingParticipantDetail);
            }

            return existingParticipants;
        }

        public List<NewParticipant> ExtractNewParticipants(VideoHearing hearing, List<EditableParticipantRequest> participants, bool ejudFeatureEnabled)
        {
            var newParticipants = new List<NewParticipant>();

            foreach (var participant in participants)
            {
                if (participant.Id != null)
                    // Participant is existing, not new
                    continue;

                if (ProcessNewParticipant(participant, hearing, ejudFeatureEnabled) is {} newParticipantToAdd)
                {
                    var newParticipant = ParticipantRequestToNewParticipantMapper.Map(newParticipantToAdd, hearing.CaseType);
                    newParticipants.Add(newParticipant);
                }
            }

            return newParticipants;
        }
        
        private EditableParticipantRequest ProcessNewParticipant(
            EditableParticipantRequest participant,
            VideoHearing hearing,
            bool ejudFeatureEnabled)
        {
            // Add a new participant
            // Map the request except the username
            if ((ejudFeatureEnabled && (participant.CaseRoleName == "Judge" // TODO can we get these from a constant?
                                        || participant.HearingRoleName == "Panel Member"
                                        || participant.HearingRoleName == "Winger"))
                || (!ejudFeatureEnabled && participant.CaseRoleName == "Judge"))
            {
                if (hearing.Participants?.SingleOrDefault(p => p.Person.ContactEmail.Equals(participant.ContactEmail)) != null)
                {
                    //If the judge already exists in the database, there is no need to add again.
                    return null;
                }

                participant.Username = participant.ContactEmail;
            }

            // _logger.LogDebug("Adding participant {Participant} to hearing {Hearing}",
            //     newParticipant.DisplayName, hearingId);
            return participant;
        }
        
        public List<Guid> ExtractRemovedParticipantIds(VideoHearing hearing, List<EditableParticipantRequest> participants)
        {
            return hearing.Participants.Where(p => participants.TrueForAll(rp => rp.Id != p.Id))
                .Select(x => x.Id).ToList();
        }
        
        public List<LinkedParticipantDto> ExtractLinkedParticipants(VideoHearing hearing, 
            List<EditableParticipantRequest> participants,
            List<ExistingParticipantDetails> existingParticipants,
            List<NewParticipant> newParticipants)
        {
            var linkedParticipantRequests = new List<LinkedParticipantRequest>();
            var participantsWithLinks = participants
                .Where(x => x.LinkedParticipants.Any())
                .ToList();

            for (int i = 0; i < participantsWithLinks.Count; i++)
            {
                var participantWithLinks = participantsWithLinks[i];
                var linkedParticipantRequest = new LinkedParticipantRequest
                {
                    LinkedParticipantContactEmail = participantWithLinks.LinkedParticipants[0].LinkedParticipantContactEmail,
                    ParticipantContactEmail = participantWithLinks.LinkedParticipants[0].ParticipantContactEmail ?? participantWithLinks.ContactEmail,
                    Type = participantWithLinks.LinkedParticipants[0].Type
                };

                // If the participant link is not new and already existed, then the ParticipantContactEmail will be null. We find it here and populate it.
                // We also remove the participant this one is linked to, to avoid duplicating entries.
                if (participantWithLinks.Id.HasValue &&
                    existingParticipants.SingleOrDefault(x => x.ParticipantId == participantWithLinks.Id) != null)
                {
                    // Is the linked participant an existing participant?
                    var secondaryParticipantInLinkContactEmail = hearing.Participants
                        .SingleOrDefault(x => x.Person.ContactEmail == participantWithLinks.LinkedParticipants[0].LinkedParticipantContactEmail)?
                        .Person.ContactEmail ?? newParticipants
                        .SingleOrDefault(x =>
                            x.Person.ContactEmail == participantWithLinks.LinkedParticipants[0].LinkedParticipantContactEmail)?
                        .Person.ContactEmail;

                    // If the linked participant isn't an existing participant it will be a newly added participant                        
                    linkedParticipantRequest.LinkedParticipantContactEmail = secondaryParticipantInLinkContactEmail;

                    // If the linked participant is an already existing user they will be mapped twice, so we remove them here.
                    var secondaryParticipantInLinkIndex = participantsWithLinks
                        .FindIndex(x => x.ContactEmail == secondaryParticipantInLinkContactEmail);
                    if (secondaryParticipantInLinkIndex >= 0)
                        participantsWithLinks.RemoveAt(secondaryParticipantInLinkIndex);
                }
                
                linkedParticipantRequests.Add(linkedParticipantRequest);
            }

            var linkedParticipants = LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(linkedParticipantRequests);
            return linkedParticipants;
        }
        
        public void AssignParticipantIdsForEditMultiDayHearingFutureDay(VideoHearing multiDayHearingFutureDay, 
            List<EditableParticipantRequest> participants)
        {
            // For the future day hearings, the participant ids will be different
            // So we need to set their ids to null if they are new participants, or use their existing ids if they already exist
                    
            foreach (var participant in participants)
            {
                var existingParticipant = multiDayHearingFutureDay.Participants.SingleOrDefault(x => x.Person.ContactEmail == participant.ContactEmail);
                if (existingParticipant == null)
                {
                    participant.Id = null;
                            
                }
                else
                {
                    participant.Id = existingParticipant.Id;
                }
            }

            // foreach (var endpoint in endpoints)
            // {
            //     // Unlike participants we don't have a common identifier, so need to remove the existing endpoints and replace them
            //     endpoint.Id = null;
            // }
        }
    }
}
