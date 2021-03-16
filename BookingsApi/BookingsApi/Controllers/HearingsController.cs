using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.Extensions;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Queries;
using BookingsApi.Common;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BookingsApi.Mappings;
using BookingsApi.Validations;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingsController : Controller
    {
        private const string DefaultCursor = "0";
        private const int DefaultLimit = 100;

        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IHearingService _hearingService;
        private readonly ILogger _logger;

        public HearingsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher,
            IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration,
            IHearingService hearingService,
            ILogger logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _randomGenerator = randomGenerator;
            _hearingService = hearingService;
            _logger = logger;
            
            _kinlyConfiguration = kinlyConfiguration.Value;
        }

        /// <summary>
        /// Get details for a given hearing
        /// </summary>
        /// <param name="hearingId">Id for a hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{hearingId}", Name = "GetHearingDetailsById")]
        [OpenApiOperation("GetHearingDetailsById")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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

            var hearingMapper = new HearingToDetailsResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(videoHearing);
            return Ok(response);
        }

        /// <summary>
        /// Get list of all hearings for a given username
        /// </summary>
        /// <param name="username">username of person to search against</param>
        /// <returns>Hearing details</returns>
        [HttpGet(Name = "GetHearingsByUsername")]
        [OpenApiOperation("GetHearingsByUsername")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetHearingsByUsername([FromQuery]string username)
        {
            var query = new GetHearingsByUsernameQuery(username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingToDetailsResponseMapper();
            var response = hearings.Select(hearingMapper.MapHearingToDetailedResponse).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Get list of all hearings in a group
        /// </summary>
        /// <param name="groupId">the group id of the single day or multi day hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{groupId}/hearings", Name = "GetHearingsByGroupId")]
        [OpenApiOperation("GetHearingsByGroupId")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetHearingsByGroupId(Guid groupId)
        {
            var query = new GetHearingsByGroupIdQuery(groupId);
            var hearings = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingToDetailsResponseMapper();
            var response = hearings.Select(hearingMapper.MapHearingToDetailedResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [OpenApiOperation("BookNewHearing")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewHearing(BookNewHearingRequest request)
        {
            try
            {
                if (request == null)
                {
                    const string modelErrorMessage = "BookNewHearingRequest is null";
                    const string logModelErrorMessage = "BookNewHearing Error: BookNewHearingRequest is null";

                    return ModelStateErrorLogger(nameof(BookNewHearingRequest), modelErrorMessage, logModelErrorMessage,
                        null, SeverityLevel.Information);
                }
                
                var result = await new BookNewHearingRequestValidation().ValidateAsync(request);
                if (!result.IsValid)
                {
                    const string logBookNewHearingValidationError = "BookNewHearing Validation Errors";
                    const string emptyPayLoadErrorMessage = "Empty Payload";
                    const string keyPayload = "payload";

                    ModelState.AddFluentValidationErrors(result.Errors);
                    var dictionary = result.Errors.ToDictionary(x => x.PropertyName + "-" + Guid.NewGuid(), x => x.ErrorMessage);
                    var payload = JsonConvert.SerializeObject(request);
                    dictionary.Add(keyPayload, !string.IsNullOrWhiteSpace(payload) ? payload : emptyPayLoadErrorMessage);
                    _logger.TrackTrace(logBookNewHearingValidationError, SeverityLevel.Error, dictionary);
                    return BadRequest(ModelState);
                }

                var query = new GetCaseTypeQuery(request.CaseTypeName);
                var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);
                if (caseType == null)
                {
                    const string logCaseDoesNotExist = "BookNewHearing Error: Case type does not exist";
                    return ModelStateErrorLogger(nameof(request.CaseTypeName),
                        "Case type does not exist", logCaseDoesNotExist, request.CaseTypeName, SeverityLevel.Error);
                }

                var hearingType = caseType.HearingTypes.SingleOrDefault(x => x.Name == request.HearingTypeName);
                if (hearingType == null)
                {
                    const string logHearingTypeDoesNotExist = "BookNewHearing Error: Hearing type does not exist";
                    return ModelStateErrorLogger(nameof(request.HearingTypeName),
                        "Hearing type does not exist", logHearingTypeDoesNotExist, request.HearingTypeName, SeverityLevel.Error);
                }

                var venue = await GetVenue(request.HearingVenueName);
                if (venue == null)
                {
                    const string logHearingVenueDoesNotExist = "BookNewHearing Error: Hearing venue does not exist";

                    return ModelStateErrorLogger(nameof(request.HearingVenueName),
                        "Hearing venue does not exist", logHearingVenueDoesNotExist, request.HearingVenueName, SeverityLevel.Error);
                }

                var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
                const string logHasCases = "BookNewHearing got cases";
                const string keyCases = "Cases";
                _logger.TrackTrace(logHasCases, SeverityLevel.Information, new Dictionary<string, string>
                {
                    {keyCases, string.Join(", ", cases.Select(x => new {x.Name, x.Number}))}
                });

                var createVideoHearingCommand = BookNewHearingRequestToCreateVideoHearingCommandMapper.Map(
                    request, caseType, hearingType, venue, cases, _randomGenerator, _kinlyConfiguration.SipAddressStem);
                
                const string logCallingDb = "BookNewHearing Calling DB...";
                const string dbCommand = "createVideoHearingCommand";
                const string logSaveSuccess = "BookNewHearing DB Save Success";
                const string logNewHearingId = "NewHearingId";

                _logger.TrackTrace(logCallingDb, SeverityLevel.Information, new Dictionary<string, string>{{dbCommand, JsonConvert.SerializeObject(createVideoHearingCommand)}});
                await _commandHandler.Handle(createVideoHearingCommand);
                _logger.TrackTrace(logSaveSuccess, SeverityLevel.Information, new Dictionary<string, string>{{logNewHearingId, createVideoHearingCommand.NewHearingId.ToString()}});
                
                var videoHearingId = createVideoHearingCommand.NewHearingId;

                var getHearingByIdQuery = new GetHearingByIdQuery(videoHearingId);
                var queriedVideoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
                const string logRetrieveNewHearing = "BookNewHearing Retrieved new hearing from DB";
                const string keyHearingId = "HearingId";
                const string keyCaseType = "CaseType";
                const string keyParticipantCount = "Participants.Count";
                _logger.TrackTrace(logRetrieveNewHearing, SeverityLevel.Information, new Dictionary<string, string>
                {
                    {keyHearingId, queriedVideoHearing.Id.ToString()},
                    {keyCaseType, queriedVideoHearing.CaseType?.Name},
                    {keyParticipantCount, queriedVideoHearing.Participants.Count.ToString()},
                });
                
                var hearingMapper = new HearingToDetailsResponseMapper();
                var response = hearingMapper.MapHearingToDetailedResponse(queriedVideoHearing);
                const string logProcessFinished = "BookNewHearing Finished, returning response";
                _logger.TrackTrace(logProcessFinished, SeverityLevel.Information, new Dictionary<string, string> {{"response", JsonConvert.SerializeObject(response)}});
                return CreatedAtAction(nameof(GetHearingDetailsById), new { hearingId = response.Id }, response);
            }
            catch (Exception ex)
            {
                const string keyPayload = "payload";
                const string keyScheduledDateTime = "ScheduledDateTime";
                const string keyScheduledDuration = "ScheduledDuration";
                const string keyCaseTypeName = "CaseTypeName";
                const string keyHearingTypeName = "HearingTypeName";

                if (request != null)
                {
                    var payload = JsonConvert.SerializeObject(request);
                    _logger.TrackError(ex, new Dictionary<string, string>
                    {
                        {keyPayload, !string.IsNullOrWhiteSpace(payload) ? payload : "Empty Payload"},
                        {keyScheduledDateTime, request.ScheduledDateTime.ToString("s")},
                        {keyScheduledDuration, request.ScheduledDuration.ToString()},
                        {keyCaseTypeName, request.CaseTypeName},
                        {keyHearingTypeName, request.HearingTypeName}
                    });
                }

                throw;
            }
        }

        private IActionResult ModelStateErrorLogger(string key,string exception, string logErrorMessage, string errorValue, SeverityLevel severity)
        {
            ModelState.AddModelError(key, exception);
            if (errorValue == null)
            {
                _logger.TrackTrace(logErrorMessage, severity);
            }
            else
            {
               _logger.TrackTrace(logErrorMessage, severity, new Dictionary<string, string> { { key, errorValue } }); 
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new hearing with the details of a given hearing on given dates
        /// </summary>
        /// <param name="hearingId">Original hearing to clone</param>
        /// <param name="request">List of dates to create a new hearing on</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/clone")]
        [OpenApiOperation("CloneHearing")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CloneHearing([FromRoute] Guid hearingId,
            [FromBody] CloneHearingRequest request)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var validationResult =
                new CloneHearingRequestValidation(videoHearing)
                    .ValidateDates(request);
            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return BadRequest(ModelState);
            }

            var orderedDates = request.Dates.OrderBy(x => x).ToList();
            var totalDays = orderedDates.Count + 1; // include original hearing
            var commands = orderedDates.Select((newDate, index) =>
            {
                var hearingDay = index + 2; // zero index including original hearing
                return CloneHearingToCommandMapper.CloneToCommand(videoHearing, newDate, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem, totalDays, hearingDay);
            }).ToList();
            foreach (var command in commands)
            {
                // dbcontext is not thread safe. loop one at a time
                await _commandHandler.Handle(command);
            }

            var existingCase = videoHearing.GetCases().First();
            await _hearingService.UpdateHearingCaseName(hearingId, $"{existingCase.Name} Day {1} of {totalDays}");
            
            return NoContent();
        }

        /// <summary>
        /// Update the details of a hearing such as venue, time and duration
        /// </summary>
        /// <param name="hearingId">The id of the hearing to update</param>
        /// <param name="request">Details to update</param>
        /// <returns>Details of updated hearing</returns>
        [HttpPut("{hearingId}")]
        [OpenApiOperation("UpdateHearingDetails")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateHearingDetails(Guid hearingId, [FromBody] UpdateHearingRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateHearingRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var venue = await GetVenue(request.HearingVenueName);

            if (venue == null)
            {
                ModelState.AddModelError(nameof(request.HearingVenueName), "Hearing venue does not exist");
                return BadRequest(ModelState);
            }

            var cases = MapCase(request.Cases);

            // use existing video hearing values here when request properties are null
            request.AudioRecordingRequired ??= videoHearing.AudioRecordingRequired;
            request.QuestionnaireNotRequired ??= videoHearing.QuestionnaireNotRequired;
            request.HearingRoomName ??= videoHearing.HearingRoomName;
            request.OtherInformation ??= videoHearing.OtherInformation;
            
            var command = new UpdateHearingCommand(hearingId, request.ScheduledDateTime,
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, request.QuestionnaireNotRequired.Value, request.AudioRecordingRequired.Value);

            await _commandHandler.Handle(command);

            var hearingMapper = new HearingToDetailsResponseMapper();
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            var response = hearingMapper.MapHearingToDetailedResponse(updatedHearing);

            if (videoHearing.Status == BookingStatus.Created)
            {
                // publish this event when Hearing is set for ready for video
                await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(updatedHearing));
            }

            return Ok(response);
        }

        /// <summary>
        /// Remove an existing hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}")]
        [OpenApiOperation("RemoveHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveHearing(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound($"{hearingId} does not exist");
            }

            var command = new RemoveHearingCommand(hearingId);

            await _commandHandler.Handle(command);

            if (videoHearing.Status == BookingStatus.Created)
            {
                // publish the event only for confirmed(created) hearing  
                await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(hearingId));
            }
            return NoContent();
        }

        /// <summary>
        /// Update booking status
        /// </summary>
        /// <param name="hearingId">Id of the hearing to update the status for</param>
        /// <param name="request">Status of the hearing to change to</param>
        /// <returns>Success status</returns>
        [HttpPatch("{hearingId}")]
        [OpenApiOperation("UpdateBookingStatus")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UpdateBookingStatus(Guid hearingId, UpdateBookingStatusRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateBookingStatusRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            try
            {
                var bookingStatus = Enum.Parse<BookingStatus>(request.Status.ToString());
                await UpdateHearingStatusAsync(hearingId, bookingStatus, request.UpdatedBy, request.CancelReason);

                switch (bookingStatus)
                {
                    case BookingStatus.Created:
                        var queriedVideoHearing = await GetHearingToPublishAsync(hearingId);
                        await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(queriedVideoHearing));
                        break;
                    case BookingStatus.Cancelled:
                        await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(hearingId));
                        break;
                }

                return NoContent();
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }
            catch (DomainRuleException exception)
            {
                exception.ValidationFailures.ForEach(x => ModelState.AddModelError(x.Name, x.Message));
                return Conflict(ModelState);
            }
        }

        private async Task UpdateHearingStatusAsync(Guid hearingId, BookingStatus bookingStatus, string updatedBy, string cancelReason)
        {
            var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updatedBy, cancelReason);

            await _commandHandler.Handle(command);
        }

        /// <summary>
        ///     Get a paged list of booked hearings
        /// </summary>
        /// <param name="types">The hearing case types.</param>
        /// <param name="cursor">Cursor specifying from which entries to read next page, is defaulted if not specified</param>
        /// <param name="limit">The max number hearings records to return.</param>
        /// <returns>The list of bookings video hearing</returns>
        [HttpGet("types", Name = "GetHearingsByTypes")]
        [OpenApiOperation("GetHearingsByTypes")]
        [ProducesResponseType(typeof(BookingsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BookingsResponse>> GetHearingsByTypes([FromQuery(Name = "types")]List<int> types, [FromQuery]string cursor = DefaultCursor, [FromQuery]int limit = DefaultLimit)
        {
            types = types ?? new List<int>();
            if (!await ValidateCaseTypes(types))
            {
                ModelState.AddModelError("Hearing types", "Invalid value for hearing types");
                return BadRequest(ModelState);
            }

            var query = new GetBookingsByCaseTypesQuery(types)
            {
                Cursor = cursor == DefaultCursor ? null : cursor,
                Limit = limit
            };
            var result = await _queryHandler.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(query);

            var mapper = new VideoHearingsToBookingsResponseMapper();

            var response = new BookingsResponse
            {
                PrevPageUrl = BuildCursorPageUrl(cursor, limit, types),
                NextPageUrl = BuildCursorPageUrl(result.NextCursor, limit, types),
                NextCursor = result.NextCursor,
                Limit = limit,
                Hearings = mapper.MapHearingResponses(result)
            };

            return Ok(response);
        }

        /// <summary>
        /// Anonymises the Hearings, Case, Person and Participant data.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("anonymisehearings")]
        [OpenApiOperation("AnonymiseHearings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AnonymiseHearingsAsync()
        {
            var anonymiseHearingsCommand = new AnonymiseHearingsCommand();
            await _commandHandler.Handle(anonymiseHearingsCommand);
            return NoContent();
        }

        private async Task<Hearing> GetHearingToPublishAsync(Guid hearingId)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            return videoHearing;
        }

        private async Task<HearingVenue> GetVenue(string venueName)
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);
            return hearingVenues.SingleOrDefault(x => x.Name == venueName);
        }

        private string BuildCursorPageUrl(string cursor, int limit, List<int> caseTypes)
        {
            const string hearingsListsEndpointBaseUrl = "hearings/";
            const string bookingsEndpointUrl = "types";
            const string resourceUrl = hearingsListsEndpointBaseUrl + bookingsEndpointUrl;

            var types = string.Empty;
            if (caseTypes.Any())
            {
                types = string.Join("&types=", caseTypes);
            }

            return $"{resourceUrl}?types={types}&cursor={cursor}&limit={limit}";
        }

        private async Task<bool> ValidateCaseTypes(List<int> filterCaseTypes)
        {
            if (!filterCaseTypes.Any())
            {
                return true;
            }

            var query = new GetAllCaseTypesQuery();
            var validCaseTypes = (await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query))
                .Select(caseType => caseType.Id);

            return filterCaseTypes.All(caseType => validCaseTypes.Contains(caseType));

        }

        private List<Case> MapCase(List<CaseRequest> caseRequestList)
        {
            var cases = caseRequestList ?? new List<CaseRequest>();
            return cases.Select(caseRequest => new Case(caseRequest.Number, caseRequest.Name)).ToList();
        }


        /// <summary>
        /// Search for hearings by case number. Search will apply fuzzy matching
        /// </summary>
        /// <param name="searchQuery">Search criteria</param>
        /// <returns>list of hearings matching search criteria</returns>
        [HttpGet("audiorecording/search", Name = "SearchForHearings")]
        [OpenApiOperation("SearchForHearings")]
        [ProducesResponseType(typeof(List<AudioRecordedHearingsBySearchResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SearchForHearingsAsync([FromQuery]SearchForHearingsQuery searchQuery)
        {
            var caseNumber = WebUtility.UrlDecode(searchQuery.CaseNumber);

            var query = new GetHearingsBySearchQuery(caseNumber, searchQuery.Date);
            var hearings = await _queryHandler.Handle<GetHearingsBySearchQuery, List<VideoHearing>>(query);

            var hearingMapper = new AudioRecordedHearingsBySearchResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(hearings, caseNumber);
            return Ok(response);
        }
    }
}