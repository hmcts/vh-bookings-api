using BookingsApi.Contract.V2.Enums;
using BookingsApi.Infrastructure.Services;

namespace BookingsApi.Services
{
    public interface IEndpointService
    {
        /// <summary>
        /// Add a JVS endpoint to a hearing
        /// </summary>
        /// <param name="hearingId"></param>
        /// <param name="newEndpoint"></param>
        /// <returns></returns>
        Task<Endpoint> AddEndpoint(Guid hearingId, NewEndpoint newEndpoint);

        /// <summary>
        /// Update an endpoint
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="id"></param>
        /// <param name="defenceAdvocateContactEmail"></param>
        /// <param name="displayName"></param>
        /// <param name="languageCode"></param>
        /// <param name="otherLanguage"></param>
        /// <returns></returns>
        [Obsolete("Use the overload that includes a ScreeningDto")]
        Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName,
            string languageCode, string otherLanguage);
        
        /// <summary>
        /// Update an endpoint (V2)
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="id"></param>
        /// <param name="defenceAdvocateContactEmail"></param>
        /// <param name="displayName"></param>
        /// <param name="languageCode"></param>
        /// <param name="otherLanguage"></param>
        /// <param name="screeningDto"></param>
        /// <returns></returns>
        Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName,
            string languageCode, string otherLanguage, ScreeningDto screeningDto);
        
        /// <summary>
        /// Remove an endpoint from a hearing
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task RemoveEndpoint(VideoHearing hearing, Guid id);
        
        /// <summary>
        /// Get the SIP address stem for a supplier
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        string GetSipAddressStem(BookingSupplier? supplier);
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
                await _eventPublisher.PublishAsync(new EndpointAddedIntegrationEvent(updatedHearing, endpoint));
            }

            return endpoint;
        }

        public async Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName, string languageCode, string otherLanguage)
        {
            await UpdateEndpoint(hearing, id, defenceAdvocateContactEmail, displayName, languageCode, otherLanguage, null);
        }

        public async Task UpdateEndpoint(VideoHearing hearing, Guid id, string defenceAdvocateContactEmail, string displayName,
            string languageCode, string otherLanguage, ScreeningDto screeningDto)
        {
            var defenceAdvocate =
                DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(defenceAdvocateContactEmail,
                    hearing.GetParticipants());
            var command = new UpdateEndPointOfHearingCommand(hearing.Id, id, displayName, defenceAdvocate, languageCode,
                otherLanguage, screeningDto);
            await _commandHandler.Handle(command);

            var endpoint = command.UpdatedEndpoint;

            if (endpoint != null && (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge))
            {
               var conferenceRole = endpoint.GetEndpointConferenceRole(hearing.GetParticipants(), hearing.GetEndpoints());
                await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearing.Id, endpoint.Sip,
                    displayName, defenceAdvocate?.Person.ContactEmail, conferenceRole));
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

        public string GetSipAddressStem(BookingSupplier? supplier)
        {
            if (supplier.HasValue)
            {
                return supplier == BookingSupplier.Vodafone
                    ? _supplierConfiguration.SipAddressStemVodafone
                    : _supplierConfiguration.SipAddressStemKinly;
            }

            return _featureToggles.UseVodafoneToggle()
                ? _supplierConfiguration.SipAddressStemVodafone
                : _supplierConfiguration.SipAddressStemKinly;
        }
    }
}
