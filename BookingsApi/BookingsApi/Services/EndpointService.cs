using BookingsApi.Domain.Dtos;

namespace BookingsApi.Services
{
    public interface IEndpointService
    {
        Task<Endpoint> AddEndpoint(Guid hearingId, EndpointDto endpointDto);
        Task UpdateEndpoint(VideoHearing hearing, Guid id, List<EndpointParticipantDto> endpointParticipantDtos, string displayName);
        Task RemoveEndpoint(VideoHearing hearing, Guid id);
    }
    
    public class EndpointService : IEndpointService
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;

        public EndpointService(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
        }
        
        public async Task<Endpoint> AddEndpoint(Guid hearingId, EndpointDto endpointDto)
        {
            var command = new AddEndPointToHearingCommand(hearingId, endpointDto);
            await _commandHandler.Handle(command);

            var updatedHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            var endpoint = updatedHearing.GetEndpoints().First(x => x.DisplayName.Equals(endpointDto.DisplayName));

            if (updatedHearing.Status == BookingStatus.Created || updatedHearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointAddedIntegrationEvent(updatedHearing.Id, endpoint));
            }

            return endpoint;
        }

        public async Task UpdateEndpoint(VideoHearing hearing, Guid id, List<EndpointParticipantDto> endpointParticipantDtos, string displayName)
        {
            var endpointParticipants =  hearing
                .GetParticipants()
                .GetEndpointParticipants(endpointParticipantDtos);
            
            var command = new UpdateEndPointOfHearingCommand(hearing.Id, id, displayName, endpointParticipants.ToArray());
            await _commandHandler.Handle(command);

            var endpoint = hearing.GetEndpoints().SingleOrDefault(x => x.Id == id);

            if (endpoint != null && (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge))
            {
                await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearing.Id, endpoint.Sip, displayName));
            }
        }

        public async Task RemoveEndpoint(VideoHearing hearing, Guid id)
        {
            var command = new RemoveEndPointFromHearingCommand(hearing.Id, id);
            await _commandHandler.Handle(command);
            var ep = hearing.GetEndpoints().First(x => x.Id == id);
            if (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointRemovedIntegrationEvent(hearing.Id, ep.Sip));
            }
        }
    }
}
