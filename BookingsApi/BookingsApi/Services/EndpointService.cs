namespace BookingsApi.Services
{
    public interface IEndpointService
    {
        Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEndpoint);

        Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName,
            string languageCode, string otherLanguage);
        Task RemoveEndpoint(VideoHearing hearing, Guid id);
        
        string GetSipAddressStem();
    }
    
    public class EndpointService : IEndpointService
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly SupplierConfiguration _supplierConfiguration;
        private readonly IFeatureToggles _featureToggles;

        public EndpointService(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher, IOptions<SupplierConfiguration> supplierConfiguration, IFeatureToggles featureToggles)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _supplierConfiguration = supplierConfiguration.Value;
            _featureToggles = featureToggles;
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

        public async Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName, string languageCode, string otherLanguage)
        {
            var defenceAdvocate =
                DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(defenceAdvocateContactEmail,
                    hearing.GetParticipants());
            var command = new UpdateEndPointOfHearingCommand(hearing.Id, id, displayName, defenceAdvocate, languageCode,
                otherLanguage);
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
            var command = new RemoveEndPointFromHearingCommand(hearing.Id, id);
            await _commandHandler.Handle(command);
            var ep = hearing.GetEndpoints().First(x => x.Id == id);
            if (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(new EndpointRemovedIntegrationEvent(hearing.Id, ep.Sip));
            }
        }

        public string GetSipAddressStem()
        {
            return _featureToggles.UseVodafoneToggle()
                ? _supplierConfiguration.SipAddressStemVodafone
                : _supplierConfiguration.SipAddressStemKinly;
        }
    }
}
