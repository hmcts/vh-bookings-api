using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;
using FluentValidation;
using FluentValidation.Results;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        Task<ValidationResult> ValidateUpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, List<HearingRole> hearingRoles);
        Task<VideoHearing> UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles);
        Task<ValidationResult> ValidateUpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request);
        Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing);
        Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEndpoint);
        Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName);
        Task RemoveEndpoint(VideoHearing hearing, Guid id);
        Task<IList<JudiciaryParticipant>> AddJudiciaryParticipants(List<NewJudiciaryParticipant> newJudiciaryParticipants, Guid hearingId);
        Task<JudiciaryParticipant> UpdateJudiciaryParticipant(UpdatedJudiciaryParticipant judiciaryParticipant, Guid hearingId);
        Task RemoveJudiciaryParticipant(string personalCode, Guid hearingId);
        Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request, Guid hearingId);
        Task<ValidationResult> ValidateUpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request);
        Task<JudiciaryParticipant> ReassignJudiciaryJudge(Guid hearingId, NewJudiciaryJudge newJudiciaryJudge);
    }
    
    public class UpdateHearingService : IUpdateHearingService
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IEventPublisher _eventPublisher;

        public UpdateHearingService(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IHearingParticipantService hearingParticipantService, IRandomGenerator randomGenerator, 
            IOptions<KinlyConfiguration> kinlyConfiguration, IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hearingParticipantService = hearingParticipantService;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfiguration.Value;
            _eventPublisher = eventPublisher;
        }
        
        public async Task<ValidationResult> ValidateUpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, List<HearingRole> hearingRoles)
        {
            var result = await new UpdateHearingParticipantsRequestInputValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                return result;
            }
            
            var dataValidationResult = await new UpdateHearingParticipantsRequestRefDataValidationV2(hearingRoles).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                return dataValidationResult;
            }
            
            return result;
        }

        public async Task<ValidationResult> ValidateUpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request)
        {
            foreach (var newEndpoint in request.NewEndpoints)
            {
                var result = await new EndpointRequestValidationV2().ValidateAsync(newEndpoint);
                if (!result.IsValid)
                {
                    return result;
                }
            }
            
            foreach (var existingEndpoint in request.ExistingEndpoints)
            {
                var result = await new EndpointRequestValidationV2().ValidateAsync(existingEndpoint);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateUpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request)
        {
            foreach (var newJudiciaryParticipant in request.NewJudiciaryParticipants)
            {
                var result = await new JudiciaryParticipantRequestValidationV2().ValidateAsync(newJudiciaryParticipant);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            foreach (var existingJudiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var existingJudiciaryParticipantValidationResult = await new UpdateJudiciaryParticipantRequestValidationV2().ValidateAsync(existingJudiciaryParticipant);
                if (!existingJudiciaryParticipantValidationResult.IsValid)
                {
                    return existingJudiciaryParticipantValidationResult;
                }
            }

            return new ValidationResult();
        }
        
        public async Task<VideoHearing> UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, 
            VideoHearing hearing, List<HearingRole> hearingRoles)
        {
            var newParticipants = request.NewParticipants
                .Select(x => ParticipantRequestV2ToNewParticipantMapper.Map(x, hearingRoles)).ToList();

            var existingParticipants = hearing.Participants
                .Where(x => request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id)).ToList();

            var existingParticipantDetails = UpdateExistingParticipantDetailsFromRequest(request, existingParticipants);

            var linkedParticipants =
                LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);

            var command = new UpdateHearingParticipantsCommand(hearing.Id, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

            await _commandHandler.Handle(command);

            var getHearingByIdQuery = new GetHearingByIdQuery(hearing.Id);
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            await _hearingParticipantService.PublishEventForUpdateParticipantsAsync(updatedHearing, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

            return updatedHearing;
        }

        public async Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request, Guid hearingId)
        {
            // TODO if the request contains a different judge, remove them from the request and reassign them instead
            
            var judiciaryParticipantsToAdd = request.NewJudiciaryParticipants
                .Select(JudiciaryParticipantRequestV2ToNewJudiciaryParticipantMapper.Map)
                .ToList();
            
            await AddJudiciaryParticipants(judiciaryParticipantsToAdd, hearingId);

            foreach (var existingJudiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var judiciaryParticipantToUpdate = UpdateJudiciaryParticipantRequestV2ToUpdatedJudiciaryParticipantMapper.Map(
                    existingJudiciaryParticipant.PersonalCode, existingJudiciaryParticipant);

                await UpdateJudiciaryParticipant(judiciaryParticipantToUpdate, hearingId);
            }
            
            foreach (var removedJudiciaryParticipant in request.RemovedJudiciaryParticipantPersonalCodes)
            {
                await RemoveJudiciaryParticipant(removedJudiciaryParticipant, hearingId);
            }
        }

        public async Task<IList<JudiciaryParticipant>> AddJudiciaryParticipants(List<NewJudiciaryParticipant> newJudiciaryParticipants, Guid hearingId)
        {
            var command = new AddJudiciaryParticipantsToHearingCommand(hearingId, newJudiciaryParticipants);
            await _commandHandler.Handle(command);
            
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await PublishEventsForJudiciaryParticipantsAdded(updatedHearing, newJudiciaryParticipants);
            
            var addedParticipants = updatedHearing.JudiciaryParticipants
                .Where(x => newJudiciaryParticipants.Select(p => p.PersonalCode).Contains(x.JudiciaryPerson.PersonalCode))
                .ToList();

            return addedParticipants;
        }

        public async Task<JudiciaryParticipant> UpdateJudiciaryParticipant(UpdatedJudiciaryParticipant judiciaryParticipant, Guid hearingId)
        {
            var command = new UpdateJudiciaryParticipantCommand(hearingId, judiciaryParticipant);
            await _commandHandler.Handle(command);
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await _hearingParticipantService.PublishEventForUpdateJudiciaryParticipantAsync(hearing, judiciaryParticipant);
            
            var updatedParticipant = hearing.JudiciaryParticipants
                .FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == judiciaryParticipant.PersonalCode);

            return updatedParticipant;
        }

        public async Task RemoveJudiciaryParticipant(string personalCode, Guid hearingId)
        {
            var command = new RemoveJudiciaryParticipantFromHearingCommand(hearingId, personalCode);
            await _commandHandler.Handle(command);

            // ONLY publish this event when Hearing is set for ready for video
            var videoHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await PublishEventForJudiciaryParticipantRemoved(videoHearing, command.RemovedParticipantId.Value);
        }
        
        public async Task<JudiciaryParticipant> ReassignJudiciaryJudge(Guid hearingId, NewJudiciaryJudge newJudiciaryJudge)
        {
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));

            var oldJudge = (JudiciaryParticipant)hearing.GetJudge();
            
            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);
            await _commandHandler.Handle(command);
            
            hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            
            var newJudge = (JudiciaryParticipant)hearing.GetJudge();

            await PublishEventsForJudiciaryJudgeReassigned(hearing, oldJudge?.Id, newJudge);

            return newJudge;
        }

        public async Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing)
        {
            foreach (var endpointToAdd in request.NewEndpoints)
            {
                var newEp = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(endpointToAdd, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem);

                await AddEndpoint(hearing.Id, newEp);
            }

            foreach (var endpointToUpdate in request.ExistingEndpoints)
            {
                await UpdateEndpoint(hearing, endpointToUpdate.Id, endpointToUpdate.DefenceAdvocateContactEmail, endpointToUpdate.DisplayName);
            }

            foreach (var endpointIdToRemove in request.RemovedEndpointIds)
            {
                await RemoveEndpoint(hearing, endpointIdToRemove);
            }
        }

        public async Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEndpoint)
        {
            var command = new AddEndPointToHearingCommand(hearingId, newEndpoint);
            await _commandHandler.Handle(command);

            var updatedHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            var endpoint = updatedHearing.GetEndpoints().First(x => x.DisplayName.Equals(newEndpoint.DisplayName));

            if (updatedHearing.Status == BookingStatus.Created || updatedHearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointAddedIntegrationEvent(updatedHearing.Id, endpoint));
            }

            return endpoint;
        }

        public async Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName)
        {
            // Update endpoint
            var defenceAdvocate =
                DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(defenceAdvocateContactEmail,
                    hearing.GetParticipants());
            var command = new UpdateEndPointOfHearingCommand(hearing.Id, id, displayName, defenceAdvocate);
            await _commandHandler.Handle(command);

            var endpoint = hearing.GetEndpoints().SingleOrDefault(x => x.Id == id);

            if (endpoint != null && (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge))
            {
                await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearing.Id, endpoint.Sip,
                    displayName, defenceAdvocate?.Person.ContactEmail));
            }
        }

        public async Task RemoveEndpoint(VideoHearing hearing, Guid id)
        {
            // Remove endpoint
            var command = new RemoveEndPointFromHearingCommand(hearing.Id, id);
            await _commandHandler.Handle(command);
            var ep = hearing.GetEndpoints().First(x => x.Id == id);
            if (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointRemovedIntegrationEvent(hearing.Id, ep.Sip));
            }
        }
        
        private static List<ExistingParticipantDetails> UpdateExistingParticipantDetailsFromRequest(UpdateHearingParticipantsRequestV2 request,
            List<Participant> existingParticipants)
        {
            var existingParticipantDetails = new List<ExistingParticipantDetails>();

            foreach (var existingParticipantRequest in request.ExistingParticipants)
            {
                var existingParticipant =
                    existingParticipants.SingleOrDefault(ep => ep.Id == existingParticipantRequest.ParticipantId);

                if (existingParticipant == null)
                {
                    continue;
                }

                var existingParticipantDetail = new ExistingParticipantDetails
                {
                    DisplayName = existingParticipantRequest.DisplayName,
                    OrganisationName = existingParticipantRequest.OrganisationName,
                    ParticipantId = existingParticipantRequest.ParticipantId,
                    Person = existingParticipant.Person,
                    RepresentativeInformation = new RepresentativeInformation {Representee = existingParticipantRequest.Representee},
                    TelephoneNumber = existingParticipantRequest.TelephoneNumber,
                    Title = existingParticipantRequest.Title
                };
                existingParticipantDetail.Person.ContactEmail = existingParticipant.Person.ContactEmail;
                existingParticipantDetails.Add(existingParticipantDetail);
            }

            return existingParticipantDetails;
        }
        
        private async Task PublishEventsForJudiciaryJudgeReassigned(Hearing hearing, Guid? oldJudgeId, JudiciaryParticipant newJudge)
        {
            if (oldJudgeId == newJudge.Id)
            {
                return;
            }
            
            if (oldJudgeId != null)
            {
                await PublishEventForJudiciaryParticipantRemoved(hearing, oldJudgeId.Value);
            }
            
            await PublishEventsForJudiciaryParticipantsAdded(hearing, new List<NewJudiciaryParticipant>
            {
                new()
                {
                    DisplayName = newJudge.DisplayName,
                    PersonalCode = newJudge.JudiciaryPerson.PersonalCode,
                    HearingRoleCode = newJudge.HearingRoleCode
                }
            });
        }
        
        private async Task PublishEventForJudiciaryParticipantRemoved(Hearing hearing, Guid removedJudiciaryParticipantId)
        {
            if (hearing.Status is BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(
                    new ParticipantRemovedIntegrationEvent(hearing.Id, removedJudiciaryParticipantId));
            }
        }

        private async Task PublishEventsForJudiciaryParticipantsAdded(Hearing hearing, IEnumerable<NewJudiciaryParticipant> participants)
        {
            await _hearingParticipantService.PublishEventForNewJudiciaryParticipantsAsync(hearing, participants);
        }
    }
}
