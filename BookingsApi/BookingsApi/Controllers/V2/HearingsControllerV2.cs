using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
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

            var caseType =
                await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(new GetCaseTypeQuery(requestV2.CaseTypeServiceId));
            if (caseType == null)
            {
                ModelState.AddModelError(nameof(requestV2.CaseTypeServiceId), "Case type does not exist");
                _logger.LogTrace("CaseTypeServiceId {CaseTypeServiceId} does not exist", requestV2.CaseTypeServiceId);
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


            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(new GetHearingVenuesQuery());
            var hearingVenue = hearingVenues.SingleOrDefault(x =>
                string.Equals(x.VenueCode, requestV2.HearingVenueCode, StringComparison.CurrentCultureIgnoreCase));
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
        [OpenApiOperation("GetHearingDetailsById")]
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
        
        private async Task<Hearing> GetHearingAsync(Guid hearingId)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            return await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        }
        
        private async Task PublishEventForNewBooking(Hearing videoHearing, bool isMultiDay)
        {
            if (videoHearing.Participants.Any(x => x.HearingRole.Name == "Judge"))
            {
                // The event below handles creatign users, sending the hearing notifications to the participants if the hearing is not a multi day
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