using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Queries;
using Bookings.Common;
using Bookings.Common.Configuration;
using Bookings.Common.Services;
using Bookings.DAL.Helper;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bookings.API.Controllers
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
        [SwaggerOperation(OperationId = "GetHearingDetailsById")]
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

            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(videoHearing);
            return Ok(response);
        }

        /// <summary>
        /// Get list of all hearings for a given username
        /// </summary>
        /// <param name="username">username of person to search against</param>
        /// <returns>Hearing details</returns>
        [HttpGet(Name = "GetHearingsByUsername")]
        [SwaggerOperation(OperationId = "GetHearingsByUsername")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetHearingsByUsername([FromQuery]string username)
        {
            var query = new GetHearingsByUsernameQuery(username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearings.Select(hearingMapper.MapHearingToDetailedResponse).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "BookNewHearing")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewHearing(BookNewHearingRequest request)
        {
            try
            {
                if (request == null)
                {
                    ModelState.AddModelError(nameof(BookNewHearingRequest), "BookNewHearingRequest is null");
                    _logger.TrackTrace("BookNewHearing Error: BookNewHearingRequest is null", SeverityLevel.Information);
                    return BadRequest(ModelState);
                }
                
                var result = await new BookNewHearingRequestValidation().ValidateAsync(request);
                if (!result.IsValid)
                {
                    ModelState.AddFluentValidationErrors(result.Errors);
                    var dictionary = result.Errors.ToDictionary(x => $"{x.PropertyName}-{Guid.NewGuid()}", x => x.ErrorMessage);
                    var payload = JsonConvert.SerializeObject(request);
                    dictionary.Add("payload", !string.IsNullOrWhiteSpace(payload) ? payload : "Empty Payload");
                    _logger.TrackTrace("BookNewHearing Validation Errors", SeverityLevel.Error, dictionary);
                    return BadRequest(ModelState);
                }

                var query = new GetCaseTypeQuery(request.CaseTypeName);
                var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);
                
                if (caseType == null)
                {
                    ModelState.AddModelError(nameof(request.CaseTypeName), "Case type does not exist");
                    _logger.TrackTrace("BookNewHearing Error: Case type does not exist", SeverityLevel.Error, new Dictionary<string, string>{{"CaseTypeName", request?.CaseTypeName}});
                    return BadRequest(ModelState);
                }

                var hearingType = caseType.HearingTypes.SingleOrDefault(x => x.Name == request.HearingTypeName);
                if (hearingType == null)
                {
                    ModelState.AddModelError(nameof(request.HearingTypeName), "Hearing type does not exist");
                    _logger.TrackTrace("BookNewHearing Error: Hearing type does not exist", SeverityLevel.Error, new Dictionary<string, string>{{"HearingTypeName", request?.HearingTypeName}});
                    return BadRequest(ModelState);
                }

                var venue = await GetVenue(request.HearingVenueName);
                if (venue == null)
                {
                    ModelState.AddModelError(nameof(request.HearingVenueName), "Hearing venue does not exist");
                    _logger.TrackTrace("BookNewHearing Error: Hearing venue does not exist", SeverityLevel.Error, new Dictionary<string, string>{{"HearingVenueName", request?.HearingVenueName}});
                    return BadRequest(ModelState);
                }

                var mapper = new ParticipantRequestToNewParticipantMapper();
                var newParticipants = request.Participants.Select(x => mapper.MapRequestToNewParticipant(x, caseType)).ToList();
                _logger.TrackTrace("BookNewHearing mapped participants", SeverityLevel.Information, new Dictionary<string, string>
                {
                    {"Participants", string.Join(", ", newParticipants?.Select(x => x?.Person?.Username))}
                });
                
                var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
                _logger.TrackTrace("BookNewHearing got cases", SeverityLevel.Information, new Dictionary<string, string>
                {
                    {"Cases", string.Join(", ", cases?.Select(x => new {x.Name, x.Number}))}
                });

                var endpoints = new List<NewEndpoint>();
                if (request.Endpoints != null && request.Endpoints.Count > 0)
                {
                    endpoints = request.Endpoints.Select(x =>
                        EndpointToResponseMapper.MapRequestToNewEndpointDto(x, _randomGenerator,
                            _kinlyConfiguration.SipAddressStem)).ToList();
                    
                    _logger.TrackTrace("BookNewHearing mapped endpoints", SeverityLevel.Information, new Dictionary<string, string>
                    {
                        {"Endpoints", string.Join(", ", endpoints?.Select(x => new {x?.Sip, x?.DisplayName, x?.DefenceAdvocateUsername}))}
                    });
                }

                var createVideoHearingCommand = new CreateVideoHearingCommand(caseType, hearingType,
                    request.ScheduledDateTime, request.ScheduledDuration, venue, newParticipants, cases,
                    request.AudioRecordingRequired, endpoints)
                {
                    HearingRoomName = request.HearingRoomName,
                    OtherInformation = request.OtherInformation,
                    CreatedBy = request.CreatedBy
                };

                _logger.TrackTrace("BookNewHearing Calling DB...", SeverityLevel.Information, new Dictionary<string, string>{{"createVideoHearingCommand", JsonConvert.SerializeObject(createVideoHearingCommand)}});
                await _commandHandler.Handle(createVideoHearingCommand);
                _logger.TrackTrace("BookNewHearing DB Save Success", SeverityLevel.Information, new Dictionary<string, string>{{"NewHearingId", createVideoHearingCommand.NewHearingId.ToString()}});
                
                var videoHearingId = createVideoHearingCommand.NewHearingId;

                var getHearingByIdQuery = new GetHearingByIdQuery(videoHearingId);
                var queriedVideoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
                _logger.TrackTrace("BookNewHearing Retrieved new hearing from DB", SeverityLevel.Information, new Dictionary<string, string>
                {
                    {"HearingId", queriedVideoHearing.Id.ToString()},
                    {"CaseType", queriedVideoHearing.CaseType?.Name},
                    {"Participants.Count", queriedVideoHearing.Participants.Count.ToString()},
                });

                var hearingMapper = new HearingToDetailResponseMapper();
                var response = hearingMapper.MapHearingToDetailedResponse(queriedVideoHearing);
                _logger.TrackTrace("BookNewHearing Finished, returning response", SeverityLevel.Information, new Dictionary<string, string> {{"response", JsonConvert.SerializeObject(response)}});
                return CreatedAtAction(nameof(GetHearingDetailsById), new { hearingId = response.Id }, response);
            }
            catch (Exception ex)
            {
                if (request != null)
                {
                    var payload = JsonConvert.SerializeObject(request);
                    _logger.TrackError(ex, new Dictionary<string, string>
                    {
                        {"payload", !string.IsNullOrWhiteSpace(payload) ? payload : "Empty Payload"},
                        {"ScheduledDateTime", request.ScheduledDateTime.ToString("s")},
                        {"ScheduledDuration", request.ScheduledDuration.ToString()},
                        {"CaseTypeName", request.CaseTypeName},
                        {"HearingTypeName", request.HearingTypeName}
                    });
                }
                else
                {
                    _logger.TrackError(ex, new Dictionary<string, string> {{"payload", "BookNewHearingRequest is null"}});
                }

                throw;
            }
        }

        /// <summary>
        /// Create a new hearing with the details of a given hearing on given dates
        /// </summary>
        /// <param name="hearingId">Original hearing to clone</param>
        /// <param name="request">List of dates to create a new hearing on</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/clone")]
        [SwaggerOperation(OperationId = "CloneHearing")]
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
        [SwaggerOperation(OperationId = "UpdateHearingDetails")]
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
            request.HearingRoomName ??= videoHearing.HearingRoomName;
            request.OtherInformation ??= videoHearing.OtherInformation;
            
            var command = new UpdateHearingCommand(hearingId, request.ScheduledDateTime,
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, request.AudioRecordingRequired.Value);

            await _commandHandler.Handle(command);

            var hearingMapper = new HearingToDetailResponseMapper();
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
        [SwaggerOperation(OperationId = "RemoveHearing")]
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
        [SwaggerOperation(OperationId = "UpdateBookingStatus")]
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
                var bookingStatus = Enum.Parse<BookingStatus>(request.Status.ToString(), true);
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
        [SwaggerOperation(OperationId = "GetHearingsByTypes")]
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
        [SwaggerOperation(OperationId = "AnonymiseHearings")]
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
        [SwaggerOperation(OperationId = "SearchForHearings")]
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