namespace BookingsApi.Services
{
    public interface IHearingEndpointService
    {
        Task<Endpoint> AddEndpointToHearing(Guid hearingId, NewEndpoint newEp);
        Task UpdateEndpointOfHearing(VideoHearing hearing, Guid endpointId, string displayName, string defenceAdvocateContactEmail);
        Task RemoveEndpointFromHearing(VideoHearing hearing, Guid endpointId);
    }
    
    public class HearingEndpointService : IHearingEndpointService
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;
        private readonly IEventPublisher _eventPublisher;

        public HearingEndpointService(ICommandHandler commandHandler, 
            IQueryHandler queryHandler,
            IEventPublisher eventPublisher)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _eventPublisher = eventPublisher;
        }
        
        public async Task<Endpoint> AddEndpointToHearing(Guid hearingId, NewEndpoint newEp)
        {
            var command = new AddEndPointToHearingCommand(hearingId, newEp);
            await _commandHandler.Handle(command);

            var hearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            var endpoint = hearing.GetEndpoints().First(x => x.DisplayName.Equals(newEp.DisplayName));
            
            if (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointAddedIntegrationEvent(hearingId, endpoint));
            }

            return endpoint;
        }

        public async Task UpdateEndpointOfHearing(VideoHearing hearing,
            Guid endpointId,
            string displayName,
            string defenceAdvocateContactEmail)
        {
            var defenceAdvocate =
                DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(defenceAdvocateContactEmail,
                    hearing.GetParticipants());
            var command = new UpdateEndPointOfHearingCommand(hearing.Id, endpointId, displayName, defenceAdvocate);
            await _commandHandler.Handle(command);

            var endpoint = hearing.GetEndpoints().SingleOrDefault(x => x.Id == endpointId);

            if (endpoint != null && (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge))
            {
                await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearing.Id, endpoint.Sip,
                    displayName, defenceAdvocate?.Person.ContactEmail));
            }
        }

        public async Task RemoveEndpointFromHearing(VideoHearing hearing, Guid endpointId)
        {
            var command = new RemoveEndPointFromHearingCommand(hearing.Id, endpointId);
            await _commandHandler.Handle(command);
            var ep = hearing.GetEndpoints().First(x => x.Id == endpointId);
            if (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointRemovedIntegrationEvent(hearing.Id, ep.Sip));
            }
        }
    }
}
