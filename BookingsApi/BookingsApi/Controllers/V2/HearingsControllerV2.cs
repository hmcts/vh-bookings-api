using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Extensions;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V2
{
    [Produces("application/json")]
    [Route(template:"v{version:apiVersion}/hearings")]
    [ApiController]
    [ApiVersion("2.0")]
    public class HearingsControllerV2 : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly ILogger<HearingsControllerV2> _logger;

        public HearingsControllerV2(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher, ILogger<HearingsControllerV2> logger, IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfigurationOption)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfigurationOption.Value;
        }

        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="requestV2">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [OpenApiOperation("BookNewHearingWithCode")]
        [ProducesResponseType(typeof(HearingDetailsResponseV2), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> BookNewHearingWithCode(BookNewHearingRequestV2 requestV2)
        {
            SanitiseRequest(requestV2);
            var result = await new BookNewHearingRequestValidationV2().ValidateAsync(requestV2);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);

                return ValidationProblem(ModelState);
            }

            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(new GetCaseRolesForCaseServiceQuery(requestV2.ServiceId));
            if (caseType == null)
            {
                ModelState.AddModelError(nameof(requestV2.ServiceId), "Case type does not exist");
                _logger.LogTrace("CaseTypeServiceId {CaseTypeServiceId} does not exist", requestV2.ServiceId);
                return ValidationProblem(ModelState);
            }

            var hearingType = caseType.HearingTypes.SingleOrDefault(x =>
                string.Equals(x.Code, requestV2.HearingTypeCode, StringComparison.CurrentCultureIgnoreCase));
            if (hearingType == null)
            {
                ModelState.AddModelError(nameof(requestV2.HearingTypeCode),
                    $"Hearing type code {requestV2.HearingTypeCode} does not exist");
                _logger.LogTrace("HearingTypeCode {HearingTypeCode} does not exist", requestV2.HearingTypeCode);
                return ValidationProblem(ModelState);
            }


            var hearingVenue = await GetHearingVenue(requestV2.HearingVenueCode);
            if (hearingVenue == null)
            {
                ModelState.AddModelError(nameof(requestV2.HearingVenueCode),
                    $"Hearing venue code {requestV2.HearingVenueCode} does not exist");
                _logger.LogTrace("HearingVenueCode {HearingVenueCode} does not exist", requestV2.HearingVenueCode);
                return ValidationProblem(ModelState);
            }
            
            var cases = requestV2.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
            var createVideoHearingCommand = BookNewHearingRequestToCreateVideoHearingCommandMapper.Map(
                requestV2, caseType, hearingType, hearingVenue, cases, _randomGenerator, _kinlyConfiguration.SipAddressStem);

            await _commandHandler.Handle(createVideoHearingCommand);
            var queriedVideoHearing = await GetHearingAsync(createVideoHearingCommand.NewHearingId);
            await PublishEventForNewBooking(queriedVideoHearing, requestV2.IsMultiDayHearing);
            var response = HearingToDetailsResponseMapper.Map(queriedVideoHearing);
            return CreatedAtAction(nameof(GetHearingDetailsById), new { hearingId = response.Id }, response);
        }

        /// <summary>
        /// Get details for a given hearing
        /// </summary>
        /// <param name="hearingId">Id for a hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{hearingId}")]
        [OpenApiOperation("GetHearingDetailsByIdV2")]
        [ProducesResponseType(typeof(HearingDetailsResponseV2), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetHearingDetailsById(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var response = HearingToDetailsResponseMapper.Map(videoHearing);
            return Ok(response);
        }
        
        
        /// <summary>
        /// Update the details of a hearing such as venue, time and duration
        /// </summary>
        /// <param name="hearingId">The id of the hearing to update</param>
        /// <param name="request">Details to update</param>
        /// <returns>Details of updated hearing</returns>
        [HttpPut("{hearingId}")]
        [OpenApiOperation("UpdateHearingDetails")]
        [ProducesResponseType(typeof(HearingDetailsResponseV2), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateHearingDetails(Guid hearingId, [FromBody] UpdateHearingRequestV2 request)
        {
            var result = await new UpdateHearingRequestValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var venue = await GetHearingVenue(request.HearingVenueCode);
            if (venue == null)
            {
                ModelState.AddModelError(nameof(request.HearingVenueCode),
                    $"Hearing venue code {request.HearingVenueCode} does not exist");
                _logger.LogTrace("HearingVenueCode {HearingVenueCode} does not exist", request.HearingVenueCode);
                return ValidationProblem(ModelState);
            }

            var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();

            // use existing video hearing values here when request properties are null
            request.AudioRecordingRequired ??= videoHearing.AudioRecordingRequired;
            request.HearingRoomName ??= videoHearing.HearingRoomName;
            request.OtherInformation ??= videoHearing.OtherInformation;

            var command = new UpdateHearingCommand(hearingId, request.ScheduledDateTime,
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, false, request.AudioRecordingRequired.Value);

            await _commandHandler.Handle(command);

        
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            var response = HearingToDetailsResponseMapper.Map(updatedHearing);

            if (videoHearing.Status == BookingStatus.Created)
            {
                await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(updatedHearing));
                if (request.ScheduledDateTime.Ticks != videoHearing.ScheduledDateTime.Ticks)
                {
                    await _eventPublisher.PublishAsync(new HearingDateTimeChangedIntegrationEvent(updatedHearing, videoHearing.ScheduledDateTime));
                }
            }

            return Ok(response);
        }
        
        private async Task<HearingVenue> GetHearingVenue(string venueCode)
        {
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(new GetHearingVenuesQuery());
            var hearingVenue = hearingVenues.SingleOrDefault(x =>
                string.Equals(x.VenueCode, venueCode, StringComparison.CurrentCultureIgnoreCase));
            return hearingVenue;
        }
        
        private async Task<Hearing> GetHearingAsync(Guid hearingId)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            return await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        }
        
        private async Task PublishEventForNewBooking(Hearing videoHearing, bool isMultiDay)
        {
            if (videoHearing.Participants.Any(x => x.HearingRole.Name == "Judge"))
            {
                // The event below handles creating users, sending the hearing notifications to the participants if the hearing is not a multi day
                await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(videoHearing, videoHearing.Participants));
            }
            else
            {
                await _eventPublisher.PublishAsync(new CreateAndNotifyUserIntegrationEvent(videoHearing, videoHearing.Participants));
                if (!isMultiDay)
                {
                    await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(videoHearing, videoHearing.Participants));
                }
            }
            
        }

        private static void SanitiseRequest(BookNewHearingRequestV2 requestV2)
        {
            foreach (var participant in requestV2.Participants)
            {
                participant.FirstName = participant.FirstName?.Trim();
                participant.LastName = participant.LastName?.Trim();
            }
        }
    }
}