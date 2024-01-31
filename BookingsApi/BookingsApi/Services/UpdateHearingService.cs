using BookingsApi.Contract.V1.Requests;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        Task UpdateHearing(VideoHearing hearing, List<EditableParticipantRequest> updatedParticipants, List<EditableEndpointRequest> updatedEndpoints, bool ejudFeatureEnabled);
        void AssignParticipantIdsForEditMultiDayHearingFutureDay(VideoHearing multiDayHearingFutureDay,
            List<EditableParticipantRequest> participants, List<EditableEndpointRequest> endpoints);
    }
    
    public class UpdateHearingService : IUpdateHearingService
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IHearingEndpointService _hearingEndpointService;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;

        public UpdateHearingService(ICommandHandler commandHandler,
            IQueryHandler queryHandler,
            IHearingParticipantService hearingParticipantService,
            IHearingEndpointService hearingEndpointService,
            IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _hearingParticipantService = hearingParticipantService;
            _hearingEndpointService = hearingEndpointService;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfiguration.Value;
        }
        
        public async Task UpdateHearing(VideoHearing hearing,
            List<EditableParticipantRequest> updatedParticipants,
            List<EditableEndpointRequest> updatedEndpoints,
            bool ejudFeatureEnabled)
        {
            // Update participants
                
            var existingParticipants = ExtractExistingParticipants(hearing, updatedParticipants);
            var newParticipants = ExtractNewParticipants(hearing, updatedParticipants, ejudFeatureEnabled);
            var removedParticipantIds = ExtractRemovedParticipantIds(hearing, updatedParticipants);
            var linkedParticipants = ExtractLinkedParticipants(hearing, updatedParticipants, existingParticipants, newParticipants);
            
            var command = new UpdateHearingParticipantsCommand(hearing.Id, existingParticipants, newParticipants, removedParticipantIds, linkedParticipants);
            await _commandHandler.Handle(command);

            var getHearingQuery = new GetHearingByIdQuery(hearing.Id);
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingQuery);
            await _hearingParticipantService
                .PublishEventForUpdateParticipantsAsync(updatedHearing, existingParticipants, newParticipants, removedParticipantIds, linkedParticipants);
            
            // Update endpoints

            var newEndpoints = ExtractNewEndpoints(updatedEndpoints, _randomGenerator, _kinlyConfiguration.SipAddressStem);
            var existingEndpoints = ExtractExistingEndpoints(updatedEndpoints);
            var removedEndpointIds = ExtractRemovedEndpointIds(hearing, updatedEndpoints);
            
            foreach (var endpoint in newEndpoints)
            {
                await _hearingEndpointService.AddEndpointToHearing(hearing.Id, endpoint);
            }

            foreach (var endpoint in existingEndpoints)
            {
                await _hearingEndpointService.UpdateEndpointOfHearing(hearing, endpoint.endpointId, endpoint.displayName, endpoint.defenceAdvocateEmail);
            }

            foreach (var endpointId in removedEndpointIds)
            {
                await _hearingEndpointService.RemoveEndpointFromHearing(hearing, endpointId);
            }
        }
        
        public void AssignParticipantIdsForEditMultiDayHearingFutureDay(VideoHearing multiDayHearingFutureDay, 
            List<EditableParticipantRequest> participants, List<EditableEndpointRequest> endpoints)
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

            foreach (var endpoint in endpoints)
            {
                // Unlike participants we don't have a common identifier, so need to remove the existing endpoints and replace them
                endpoint.Id = null;
            }
        }

        private static List<ExistingParticipantDetails> ExtractExistingParticipants(Hearing hearing, List<EditableParticipantRequest> participants)
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

        private List<NewParticipant> ExtractNewParticipants(VideoHearing hearing, List<EditableParticipantRequest> participants, bool ejudFeatureEnabled)
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
        
        private static EditableParticipantRequest ProcessNewParticipant(
            EditableParticipantRequest participant,
            Hearing hearing,
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

            return participant;
        }

        private static List<Guid> ExtractRemovedParticipantIds(Hearing hearing, List<EditableParticipantRequest> participants)
        {
            return hearing.Participants.Where(p => participants.TrueForAll(rp => rp.Id != p.Id))
                .Select(x => x.Id).ToList();
        }

        private static List<LinkedParticipantDto> ExtractLinkedParticipants(Hearing hearing, 
            IEnumerable<EditableParticipantRequest> participants,
            IReadOnlyCollection<ExistingParticipantDetails> existingParticipants,
            IReadOnlyCollection<NewParticipant> newParticipants)
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

        private static List<NewEndpoint> ExtractNewEndpoints(IEnumerable<EditableEndpointRequest> endpoints, 
            IRandomGenerator randomGenerator, 
            string sipAddressStem)
        {
            var endpointsToAdd = endpoints
                .Where(e => e.Id == null)
                .Select(e => EndpointToResponseMapper.MapRequestToNewEndpointDto(e, randomGenerator, sipAddressStem))
                .ToList();

            return endpointsToAdd;
        }

        private static List<(Guid endpointId, string displayName, string defenceAdvocateEmail)> ExtractExistingEndpoints(List<EditableEndpointRequest> endpoints)
        {
            var existingEndpoints = new List<(Guid endpointId, string displayName, string defenceAdvocateEmail)>();
            
            foreach (var endpoint in endpoints)
            {
                if (endpoint.Id == null) continue;
                
                existingEndpoints.Add((endpoint.Id.Value, endpoint.DisplayName, endpoint.DefenceAdvocateContactEmail));
            }

            return existingEndpoints;
        }

        private static List<Guid> ExtractRemovedEndpointIds(Hearing hearing, List<EditableEndpointRequest> endpoints)
        {
            var endpointIdsToRemove = hearing.Endpoints
                .Where(e => endpoints.TrueForAll(re => re.Id != e.Id))
                .Select(e => e.Id)
                .ToList();
            
            return endpointIdsToRemove;
        }
    }
}
