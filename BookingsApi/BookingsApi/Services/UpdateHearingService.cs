using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;
using FluentValidation.Results;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        Task<ValidationResult> ValidateUpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, List<HearingRole> hearingRoles);
        Task<VideoHearing> UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles);
        Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing);
        Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEp);
        Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName);
        Task RemoveEndpoint(VideoHearing hearing, Guid id);
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

        public async Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing)
        {
            // Copied from EndpointsController (TODO refactor)
            
            foreach (var endpointToAdd in request.NewEndpoints)
            {
                // Add endpoint
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

        public async Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEp)
        {
            var command = new AddEndPointToHearingCommand(hearingId, newEp);
            await _commandHandler.Handle(command);

            var updatedHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            var endpoint = updatedHearing.GetEndpoints().First(x => x.DisplayName.Equals(newEp.DisplayName));

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
    }
}
