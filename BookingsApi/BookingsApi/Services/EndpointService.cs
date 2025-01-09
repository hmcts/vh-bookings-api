using BookingsApi.Contract.V2.Enums;
using BookingsApi.Infrastructure.Services;

namespace BookingsApi.Services
{
    public record UpdateEndpointDto(
        string DefenceAdvocateContactEmail,
        string DisplayName,
        string LanguageCode,
        string OtherLanguage,
        string ExternalReferenceId,
        string MeasuresExternalId,
        ScreeningDto ScreeningDto);
    
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
        /// Update an endpoint (V2)
        /// </summary>
        /// <param name="hearing"></param>
        /// <param name="id"></param>
        /// <param name="updateEndpointDto"></param>
        /// <returns></returns>
        Task UpdateEndpoint(VideoHearing hearing, Guid id, UpdateEndpointDto updateEndpointDto);
        
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

        public EndpointService(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher, IOptions<SupplierConfiguration> supplierConfiguration)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _supplierConfiguration = supplierConfiguration.Value;
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
                await PublishUpdateHearingEvent(hearingId);
            }

            return endpoint;
        }

        public async Task UpdateEndpoint(VideoHearing hearing, Guid id, UpdateEndpointDto updateEndpointDto)
        {
            var defenceAdvocate =
                DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(updateEndpointDto.DefenceAdvocateContactEmail,
                    hearing.GetParticipants());
            var command = new UpdateEndPointOfHearingCommand(hearing.Id, id, updateEndpointDto.DisplayName,
                defenceAdvocate, updateEndpointDto.LanguageCode,
                updateEndpointDto.OtherLanguage, updateEndpointDto.ScreeningDto, updateEndpointDto.ExternalReferenceId, updateEndpointDto.MeasuresExternalId);
            await _commandHandler.Handle(command);

            var endpoint = command.UpdatedEndpoint;

            if (endpoint != null && (hearing.Status == BookingStatus.Created || hearing.Status == BookingStatus.ConfirmedWithoutJudge))
            {
               var conferenceRole = endpoint.GetEndpointConferenceRole(hearing.GetParticipants(), hearing.GetEndpoints());
                await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearing.Id, endpoint.Sip,
                    updateEndpointDto.DisplayName, defenceAdvocate?.Person.ContactEmail, conferenceRole));
                await PublishUpdateHearingEvent(hearing.Id);
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
                await PublishUpdateHearingEvent(hearing.Id);
            }
        }

        /// <summary>
        /// Changing endpoint or participant list may result in a room type change if there's a screening requirement change.
        /// It's simpler to publish the hearing details changed event to ensure the room type is recalculated.
        /// </summary>
        /// <param name="hearingId"></param>
        private async Task PublishUpdateHearingEvent(Guid hearingId)
        {
            var updatedHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(updatedHearing));
        }

        public string GetSipAddressStem(BookingSupplier? supplier) => _supplierConfiguration.SipAddressStemVodafone;
    }
}
