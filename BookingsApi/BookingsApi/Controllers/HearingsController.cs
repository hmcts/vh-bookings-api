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
using BookingsApi.DAL.Services;
using BookingsApi.Services;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingsController : Controller
    {

        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IHearingService _hearingService;
        private readonly IFeatureToggles _featureToggles;
        private readonly ILogger _logger;
        private readonly IHearingAllocationService _hearingAllocationService;

        public HearingsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher,
            IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration,
            IHearingService hearingService,
            IFeatureToggles featureToggles,
            ILogger logger,
            IHearingAllocationService hearingAllocationService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _randomGenerator = randomGenerator;
            _hearingService = hearingService;
            _featureToggles = featureToggles;
            _logger = logger;
            _hearingAllocationService = hearingAllocationService;

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
        public async Task<IActionResult> GetHearingsByUsername([FromQuery] string username)
        {
            var query = new GetHearingsByUsernameQuery(username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingToDetailsResponseMapper();
            var response = hearings.Select(hearingMapper.MapHearingToDetailedResponse).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Anonymise participant and case from expired hearing
        /// </summary>
        /// <param name="hearingIds">hearing ids to anonymise data with</param>
        /// <returns></returns>
        [HttpPatch("hearingids/{hearingIds}/anonymise-participant-and-case",
            Name = "AnonymiseParticipantAndCaseByHearingId")]
        [OpenApiOperation("AnonymiseParticipantAndCaseByHearingId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AnonymiseParticipantAndCaseByHearingId(List<Guid> hearingIds)
        {
            await _commandHandler.Handle(new AnonymiseCaseAndParticipantCommand { HearingIds = hearingIds });
            return Ok();
        }

        /// <summary>
        /// Get list of all hearings in a group
        /// </summary>
        /// <param name="groupId">the group id of the single day or multi day hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{groupId}/hearings", Name = "GetHearingsByGroupId")]
        [OpenApiOperation("GetHearingsByGroupId")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHearingsByGroupId(Guid groupId)
        {
            var query = new GetHearingsByGroupIdQuery(groupId);
            var hearings = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingToDetailsResponseMapper();
            var response = hearings.Select(hearingMapper.MapHearingToDetailedResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Get list of all hearings for notification between next 48 to 72 hrs. 
        /// </summary>
        /// <returns>Hearing details</returns>
        [HttpGet("notifications/gethearings", Name = "GetHearingsForNotification")]
        [OpenApiOperation("GetHearingsForNotification")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHearingsForNotificationAsync()
        {

            var query = new GetHearingsForNotificationsQuery();

            var hearings = await _queryHandler.Handle<GetHearingsForNotificationsQuery, List<VideoHearing>>(query);

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

                SanitiseRequest(request);

                var rDataFlag = _featureToggles.ReferenceDataToggle();
                var result = await new BookNewHearingRequestValidation(rDataFlag).ValidateAsync(request);
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

                var queryValue = rDataFlag ? request.CaseTypeServiceId : request.CaseTypeName;
                var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(new GetCaseTypeQuery(queryValue));
                if (caseType == null)
                {
                    const string logCaseDoesNotExist = "BookNewHearing Error: Case type does not exist";
                    return ModelStateErrorLogger(rDataFlag ? nameof(request.CaseTypeServiceId) : nameof(request.CaseTypeName), "Case type does not exist", logCaseDoesNotExist, queryValue, SeverityLevel.Error);
                }

                var hearingTypeQueryValue = rDataFlag ? request.HearingTypeCode : request.HearingTypeName;
                var hearingType = rDataFlag ? caseType.HearingTypes.SingleOrDefault(x => x.Code == hearingTypeQueryValue)
                        : caseType.HearingTypes.SingleOrDefault(x => x.Name == hearingTypeQueryValue);
                if (hearingType == null)
                {
                    const string logHearingTypeDoesNotExist = "BookNewHearing Error: Hearing type does not exist";
                    return ModelStateErrorLogger(rDataFlag ? nameof(request.HearingTypeCode) : nameof(request.HearingTypeName), "Hearing type does not exist", logHearingTypeDoesNotExist, hearingTypeQueryValue, SeverityLevel.Error);
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

                _logger.TrackTrace(logCallingDb, SeverityLevel.Information, new Dictionary<string, string> { { dbCommand, JsonConvert.SerializeObject(createVideoHearingCommand) } });
                await _commandHandler.Handle(createVideoHearingCommand);
                _logger.TrackTrace(logSaveSuccess, SeverityLevel.Information, new Dictionary<string, string> { { logNewHearingId, createVideoHearingCommand.NewHearingId.ToString() } });
                var queriedVideoHearing = await GetHearingAsync(createVideoHearingCommand.NewHearingId);
                await PublishEventForNewBooking(queriedVideoHearing, request.IsMultiDayHearing);

                const string logRetrieveNewHearing = "BookNewHearing Retrieved new hearing from DB";
                const string keyHearingId = "HearingId";
                const string keyCaseType = "CaseType";
                const string keyParticipantCount = "Participants.Count";
                var logTrace = new Dictionary<string, string>
                {
                    {keyHearingId, queriedVideoHearing.Id.ToString()},
                    {keyCaseType, queriedVideoHearing.CaseType?.Name},
                    {keyParticipantCount, queriedVideoHearing.Participants.Count.ToString()},
                };
                if (rDataFlag)
                    logTrace.Add("CaseTypeServiceId", queriedVideoHearing.CaseType?.ServiceId);
                _logger.TrackTrace(logRetrieveNewHearing, SeverityLevel.Information, logTrace);
                
                var hearingMapper = new HearingToDetailsResponseMapper();
                var response = hearingMapper.MapHearingToDetailedResponse(queriedVideoHearing);
                const string logProcessFinished = "BookNewHearing Finished, returning response";
                _logger.TrackTrace(logProcessFinished, SeverityLevel.Information, new Dictionary<string, string> { { "response", JsonConvert.SerializeObject(response) } });

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
                    var errorLog = new Dictionary<string, string>
                    {
                        {keyPayload, !string.IsNullOrWhiteSpace(payload) ? payload : "Empty Payload"},
                        {keyScheduledDateTime, request.ScheduledDateTime.ToString("s")},
                        {keyScheduledDuration, request.ScheduledDuration.ToString()},
                        {keyCaseTypeName, request.CaseTypeName},
                        {keyHearingTypeName, request.HearingTypeName}
                    };
                    if (_featureToggles.ReferenceDataToggle())
                    {
                        errorLog.Add("CaseTypeServiceId", request.CaseTypeServiceId);
                        errorLog.Add("HearingTypeCode", request.HearingTypeCode);
                    }
                    _logger.TrackError(ex, errorLog);
                }
                throw;
            }
        }

        private async Task PublishEventForNewBooking(Hearing videoHearing, bool isMultiDay)
        {
            if (videoHearing.Participants.Any(x => x.HearingRole.Name == "Judge"))
            {
                // Confirm the hearing
                await UpdateHearingStatusAsync(videoHearing.Id, BookingStatus.Created, "System", string.Empty);
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

        private IActionResult ModelStateErrorLogger(string key, string exception, string logErrorMessage, string errorValue, SeverityLevel severity)
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CloneHearing([FromRoute] Guid hearingId,
            [FromBody] CloneHearingRequest request)
        {
            var videoHearing = await GetHearingAsync(hearingId);
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

            var existingCase = videoHearing.GetCases().First();
            await _hearingService.UpdateHearingCaseName(hearingId, $"{existingCase.Name} Day {1} of {totalDays}");

            foreach (var command in commands)
            {
                // dbcontext is not thread safe. loop one at a time
                await _commandHandler.Handle(command);
                await PublishEventForNewBooking(await GetHearingAsync(command.NewHearingId), true);
            }
            
            // publish multi day hearing notification event
            await _eventPublisher.PublishAsync(new MultiDayHearingIntegrationEvent(videoHearing, totalDays));

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
                await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(updatedHearing));
                if (request.ScheduledDateTime.Ticks != videoHearing.ScheduledDateTime.Ticks)
                {
                    await _eventPublisher.PublishAsync(new HearingDateTimeChangedIntegrationEvent(updatedHearing, videoHearing.ScheduledDateTime));
                }
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
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            if (videoHearing == null)
            {
                return NotFound($"{hearingId} does not exist");
            }
            
            try
            {
                var bookingStatus = Enum.Parse<BookingStatus>(request.Status.ToString());
                if (videoHearing.Status != bookingStatus)
                {
                    await UpdateStatus(hearingId, request.UpdatedBy, request.CancelReason, bookingStatus);
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

        private async Task UpdateStatus(Guid hearingId, string updatedBy, string cancelReason, BookingStatus bookingStatus)
        {
            await UpdateHearingStatusAsync(hearingId, bookingStatus, updatedBy, cancelReason);

            switch (bookingStatus)
            {
                case BookingStatus.Cancelled:
                    await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(hearingId));
                    break;
            }
        }

        private async Task UpdateHearingStatusAsync(Guid hearingId, BookingStatus bookingStatus, string updatedBy, string cancelReason)
        {
            var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updatedBy, cancelReason);

            await _commandHandler.Handle(command);
        }

        [HttpGet("types", Name = "GetHearingsByTypes")]
        [OpenApiOperation("GetHearingsByTypes")]
        [ProducesResponseType(typeof(BookingsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BookingsResponse>> GetHearingsByTypes([FromBody] GetHearingRequest request)
        {
            request.FromDate ??= DateTime.UtcNow.Date;
            request.Types ??= new List<int>();

            if (!await ValidateCaseTypes(request.Types))
            {
                ModelState.AddModelError("Hearing types", "Invalid value for hearing types");
                return BadRequest(ModelState);
            }

            request.VenueIds ??= new List<int>();
            if (!await ValidateVenueIds(request.VenueIds))
            {
                ModelState.AddModelError("Venue ids", "Invalid value for venue ids");
                return BadRequest(ModelState);
            }

            var query = new GetBookingsByCaseTypesQuery(request.Types)
            {
                Cursor = request.Cursor == GetHearingRequest.DefaultCursor ? null : request.Cursor,
                Limit = request.Limit,
                StartDate = request.FromDate.Value,
                EndDate = request.EndDate,
                CaseNumber = request.CaseNumber,
                VenueIds = request.VenueIds,
                LastName = request.LastName,
                NoJudge = request.NoJudge
            };
            var result = await _queryHandler.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(query);

            var mapper = new VideoHearingsToBookingsResponseMapper();

            var response = new BookingsResponse
            {
                PrevPageUrl = BuildCursorPageUrl(request.Cursor, request.Limit, request.Types, request.CaseNumber, request.VenueIds, request.LastName),
                NextPageUrl = BuildCursorPageUrl(result.NextCursor, request.Limit, request.Types, request.CaseNumber, request.VenueIds, request.LastName),
                NextCursor = result.NextCursor,
                Limit = request.Limit,
                Hearings = mapper.MapHearingResponses(result)
            };

            return Ok(response);
        }

        /// <summary>
        /// Automatically allocates a user to a hearing
        /// </summary>
        /// <param name="hearingId">Id of the hearing to allocate to</param>
        /// <returns>Details of the allocated user</returns>
        [HttpPost("{hearingId}/allocations/automatic")]
        [OpenApiOperation("AllocateHearingAutomatically")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AllocateHearingAutomatically(Guid hearingId)
        {
            try
            {
                var justiceUser = await _hearingAllocationService.AllocateAutomatically(hearingId);
                
                if (justiceUser == null)
                    return NotFound();

                var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);

                return Ok(justiceUserResponse);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }
        }

        private async Task<Hearing> GetHearingAsync(Guid hearingId)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            return await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        }

        private async Task<HearingVenue> GetVenue(string venueName)
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);
            return hearingVenues.SingleOrDefault(x => x.Name == venueName);
        }

        private string BuildCursorPageUrl(
            string cursor,
            int limit,
            List<int> caseTypes,
            string caseNumber = "",
            List<int> hearingVenueIds = null,
            string lastName = "")
        {
            const string hearingsListsEndpointBaseUrl = "hearings/";
            const string bookingsEndpointUrl = "types";
            const string resourceUrl = hearingsListsEndpointBaseUrl + bookingsEndpointUrl;

            var types = string.Empty;
            if (caseTypes.Any())
            {
                types = string.Join("&types=", caseTypes);
            }

            var pageUrl = $"{resourceUrl}?types={types}&cursor={cursor}&limit={limit}";

            // Executes when Admin_Search feature toggle is ON and search action is performed
            if (_featureToggles.AdminSearchToggle())
            {
                if (!string.IsNullOrWhiteSpace(caseNumber))
                {
                    pageUrl += $"&caseNumber={caseNumber}";
                }

                var venueIds = string.Empty;
                if (hearingVenueIds != null && hearingVenueIds.Any())
                {
                    venueIds = string.Join("&venueIds=", hearingVenueIds);
                }
                pageUrl += $"&venueIds={venueIds}";

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    pageUrl += $"&lastName={lastName}";
                } 
            }

            return pageUrl;
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

        private async Task<bool> ValidateVenueIds(List<int> filterVenueIds)
        {
            if (!filterVenueIds.Any())
            {
                return true;
            }

            var query = new GetHearingVenuesQuery();
            var validVenueIds = (await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query))
                .Select(venue => venue.Id);

            return filterVenueIds.All(venueId => validVenueIds.Contains(venueId));
        }

        private static List<Case> MapCase(List<CaseRequest> caseRequestList)
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
        public async Task<IActionResult> SearchForHearingsAsync([FromQuery] SearchForHearingsQuery searchQuery)
        {
            var caseNumber = WebUtility.UrlDecode(searchQuery.CaseNumber);

            var query = new GetHearingsBySearchQuery(caseNumber, searchQuery.Date);
            var hearings = await _queryHandler.Handle<GetHearingsBySearchQuery, List<VideoHearing>>(query);

            var hearingMapper = new AudioRecordedHearingsBySearchResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(hearings, caseNumber);
            return Ok(response);
        }
        
        /// <summary>
        /// Get all the unallocated hearings
        /// </summary>
        /// <returns>unallocated hearings</returns>
        [HttpGet("unallocated")]
        [OpenApiOperation("GetUnallocatedHearings")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUnallocatedHearings()
        {

            var results = await _hearingService.GetUnallocatedHearings();

            if (results.Count <= 0)
                _logger.TrackEvent("[GetUnallocatedHearings] Could not find any unallocated hearings");
            var hearingMapper = new HearingToDetailsResponseMapper();
            var response = results.Select(hearingMapper.MapHearingToDetailedResponse).ToList();
            return Ok(response);
        }

        private static void SanitiseRequest(BookNewHearingRequest request)
        {
            foreach (var participant in request.Participants)
            {
                participant.FirstName = participant.FirstName?.Trim();
                participant.LastName = participant.LastName?.Trim();
            }
        }
    }
}
