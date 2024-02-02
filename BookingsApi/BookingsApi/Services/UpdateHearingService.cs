using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;
using FluentValidation.Results;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        Task<VideoHearing> UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles);
        Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEndpoint);
        Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName);
        Task RemoveEndpoint(VideoHearing hearing, Guid id);
        Task<IList<JudiciaryParticipant>> AddJudiciaryParticipants(List<NewJudiciaryParticipant> newJudiciaryParticipants, Guid hearingId);
        Task<JudiciaryParticipant> UpdateJudiciaryParticipant(UpdatedJudiciaryParticipant judiciaryParticipant, Guid hearingId);
        Task RemoveJudiciaryParticipant(string personalCode, Guid hearingId);
        Task<JudiciaryParticipant> ReassignJudiciaryJudge(Guid hearingId, NewJudiciaryJudge newJudiciaryJudge);
    }
    
    public class UpdateHearingService : IUpdateHearingService
    {
        public IOptions<KinlyConfiguration> KinlyConfiguration { get; }
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IEventPublisher _eventPublisher;

        public UpdateHearingService(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IHearingParticipantService hearingParticipantService, 
            IOptions<KinlyConfiguration> kinlyConfiguration, IEventPublisher eventPublisher)
        {
            KinlyConfiguration = kinlyConfiguration;
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hearingParticipantService = hearingParticipantService;
            _eventPublisher = eventPublisher;
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
