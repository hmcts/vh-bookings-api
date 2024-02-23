using BookingsApi.Contract.V1.Queries;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Services;
using BookingsApi.Helpers;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;
using FluentValidation;
using Microsoft.ApplicationInsights.DataContracts;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class HearingsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IBookingService _bookingService;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IHearingService _hearingService;
        private readonly IVhLogger _ivhLogger;
        private readonly IUpdateHearingService _updateHearingService;

        public HearingsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IBookingService bookingService,
            IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration,
            IHearingService hearingService,
            IVhLogger ivhLogger,
            IUpdateHearingService updateHearingService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _bookingService = bookingService;
            _randomGenerator = randomGenerator;
            _hearingService = hearingService;
            _ivhLogger = ivhLogger;

            _kinlyConfiguration = kinlyConfiguration.Value;
            _updateHearingService = updateHearingService;
        }

        /// <summary>
        /// Get details for a given hearing
        /// </summary>
        /// <param name="hearingId">Id for a hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{hearingId}")]
        [OpenApiOperation("GetHearingDetailsById")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingDetailsById(Guid hearingId)
        {
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
        /// Get list of all hearings for a given username
        /// </summary>
        /// <param name="username">username of person to search against</param>
        /// <returns>Hearing details</returns>
        [HttpGet]
        [OpenApiOperation("GetHearingsByUsername")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingsByUsername([FromQuery] string username)
        {
            var query = new GetHearingsByUsernameQuery(username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);
            var response = hearings.Select(HearingToDetailsResponseMapper.Map).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Get list of all confirmed hearings for a given username for today
        /// </summary>
        /// <param name="username">username of person to search against</param>
        /// <returns>Hearing details</returns>
        [HttpGet("today/username")]
        [OpenApiOperation("GetConfirmedHearingsByUsernameForToday")]
        [ProducesResponseType(typeof(List<ConfirmedHearingsTodayResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetConfirmedHearingsByUsernameForToday([FromQuery] string username)
        {
            var query = new GetConfirmedHearingsByUsernameForTodayQuery(username);
            var hearings =
                await _queryHandler.Handle<GetConfirmedHearingsByUsernameForTodayQuery, List<VideoHearing>>(query);
            if (!hearings.Any())
            {
                return NotFound($"{username.Trim().ToLower()} does not have any confirmed hearings today");
            }

            var response = hearings.Select(ConfirmedHearingsTodayResponseMapper.Map).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Anonymise participant and case from expired hearing
        /// </summary>
        /// <param name="hearingIds">hearing ids to anonymise data with</param>
        /// <returns></returns>
        [HttpPatch("anonymise-participant-and-case")]
        [OpenApiOperation("AnonymiseParticipantAndCaseByHearingId")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AnonymiseParticipantAndCaseByHearingId([FromBody] List<Guid> hearingIds)
        {
            await _commandHandler.Handle(new AnonymiseCaseAndParticipantCommand { HearingIds = hearingIds });
            return Ok();
        }

        
        /// <summary>
        /// Get list of all hearings in a group
        /// </summary>
        /// <param name="groupId">the group id of the single day or multi day hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{groupId}/hearings")]
        [OpenApiOperation("GetHearingsByGroupId")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingsByGroupId(Guid groupId)
        {
            var query = new GetHearingsByGroupIdQuery(groupId);
            var hearings = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(query);

            var response = hearings.Select(HearingToDetailsResponseMapper.Map).ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Update hearings in a multi day group
        /// </summary>
        /// <param name="groupId">The group id of the multi day hearing</param>
        /// <param name="request">List of hearings to update</param>
        /// <returns>No content</returns>
        [HttpPatch("{groupId}/hearings")]
        [OpenApiOperation("UpdateHearingsInGroup")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateHearingsInGroup(Guid groupId, [FromBody] UpdateHearingsInGroupRequest request)
        {
            var inputValidationResult = await new UpdateHearingsInGroupRequestInputValidation().ValidateAsync(request);
            if (!inputValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(inputValidationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var getHearingsByGroupIdQuery = new GetHearingsByGroupIdQuery(groupId);
            var hearingsInGroup = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(getHearingsByGroupIdQuery);
            
            if (!hearingsInGroup.Any())
            {
                return NotFound();
            }
            
            var dataValidationResult = await new UpdateHearingsInGroupRequestRefDataValidation(hearingsInGroup).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            foreach (var requestHearing in request.Hearings)
            {
                var hearing = hearingsInGroup.Single(x => x.Id == requestHearing.HearingId);
                
                var participantDataValidationResult = await new UpdateHearingParticipantsRequestRefDataValidation(hearing.CaseType).ValidateAsync(requestHearing.Participants);
                if (!participantDataValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(participantDataValidationResult.Errors);
                    return ValidationProblem(ModelState);
                }
            }
            
            foreach (var requestHearing in request.Hearings)
            {
                var hearing = hearingsInGroup.First(h => h.Id == requestHearing.HearingId);

                await _updateHearingService.UpdateParticipantsV1(requestHearing.Participants, hearing);
                
                hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(requestHearing.HearingId));
                
                await _updateHearingService.UpdateEndpointsV1(requestHearing.Endpoints, hearing);
            }

            return NoContent();
        }

        /// <summary>
        /// Cancel hearings in a multi day group
        /// </summary>
        /// <returns>No content</returns>
        [HttpPatch("{groupId}/hearings/cancel")]
        [OpenApiOperation("CancelHearingsInGroup")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CancelHearingsInGroup(Guid groupId, [FromBody] CancelHearingsInGroupRequest request)
        {
            var inputValidationResult = await new CancelHearingsInGroupRequestInputValidation().ValidateAsync(request);
            if (!inputValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(inputValidationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var getHearingsByGroupIdQuery = new GetHearingsByGroupIdQuery(groupId);
            var hearingsInGroup = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(getHearingsByGroupIdQuery);
            
            if (!hearingsInGroup.Any())
            {
                return NotFound();
            }
            
            var dataValidationResult = await new CancelHearingsInGroupRequestRefDataValidation(hearingsInGroup).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var requestHearings = hearingsInGroup.Where(h => request.HearingIds.Contains(h.Id)).ToList();
            var hearingDataValidationResult = await new CancelHearingsInGroupRequestHearingRefDataValidation(requestHearings).ValidateAsync(request);
            if (!hearingDataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(hearingDataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            foreach (var hearingId in request.HearingIds)
            {
                var hearing = hearingsInGroup.Find(h => h.Id == hearingId);

                await _bookingService.UpdateHearingStatus(hearing, BookingStatus.Cancelled, request.UpdatedBy, request.CancelReason);
            }
            
            return NoContent();
        }

        /// <summary>
        /// Get list of all hearings for notification between next 48 to 72 hrs. 
        /// </summary>
        /// <returns>Hearing details</returns>
        [HttpGet("notifications/gethearings")]
        [OpenApiOperation("GetHearingsForNotification")]
        [ProducesResponseType(typeof(List<HearingNotificationResponse>), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingsForNotificationAsync()
        {

            var query = new GetHearingsForNotificationsQuery();

            var hearings = await _queryHandler.Handle<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>(query);

            var response = hearings
                .Select(h=> new HearingNotificationResponse
                {
                    Hearing = HearingToDetailsResponseMapper.Map(h.Hearing), 
                    TotalDays = h.TotalDays,
                    SourceHearing = h.SourceHearing != null ? HearingToDetailsResponseMapper.Map(h.SourceHearing) : null
                })
                .ToList();

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
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
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

                request.SanitizeRequest();

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
                    _ivhLogger.TrackTrace(logBookNewHearingValidationError, SeverityLevel.Error, dictionary);
                    return ValidationProblem(ModelState);
                }

                var queryValue = request.CaseTypeName;
                var caseType = await _queryHandler.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(new GetCaseRolesForCaseTypeQuery(queryValue));
                if (caseType == null)
                {
                    const string logCaseDoesNotExist = "BookNewHearing Error: Case type does not exist";
                    return ModelStateErrorLogger(nameof(request.CaseTypeName), "Case type does not exist", logCaseDoesNotExist, queryValue, SeverityLevel.Error);
                }

                var hearingTypeQueryValue = request.HearingTypeName;
                var hearingType = caseType.HearingTypes.SingleOrDefault(x => x.Name == hearingTypeQueryValue);
                if (hearingType == null)
                {
                    const string logHearingTypeDoesNotExist = "BookNewHearing Error: Hearing type does not exist";
                    return ModelStateErrorLogger(nameof(request.HearingTypeName), "Hearing type does not exist", logHearingTypeDoesNotExist, hearingTypeQueryValue, SeverityLevel.Error);
                }

                var venueId = request.HearingVenueName;
                var venue = await GetVenue(venueId);
                if (venue == null)
                {
                    const string logHearingVenueDoesNotExist = "BookNewHearing Error: Hearing venue does not exist";

                    return ModelStateErrorLogger(nameof(request.HearingVenueName),
                        "Hearing venue does not exist", logHearingVenueDoesNotExist, venueId, SeverityLevel.Error);
                }

                var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
                const string logHasCases = "BookNewHearing got cases";
                const string keyCases = "Cases";
                _ivhLogger.TrackTrace(logHasCases, SeverityLevel.Information, new Dictionary<string, string>
                {
                    {keyCases, string.Join(", ", cases.Select(x => new {x.Name, x.Number}))}
                });

                var createVideoHearingCommand = BookNewHearingRequestToCreateVideoHearingCommandMapper.Map(
                    request, caseType, hearingType, venue, cases, _randomGenerator, _kinlyConfiguration.SipAddressStem);

                const string logCallingDb = "BookNewHearing Calling DB...";
                const string dbCommand = "createVideoHearingCommand";
                const string logSaveSuccess = "BookNewHearing DB Save Success";
                const string logNewHearingId = "NewHearingId";

                _ivhLogger.TrackTrace(logCallingDb, SeverityLevel.Information, new Dictionary<string, string> { { dbCommand, JsonConvert.SerializeObject(createVideoHearingCommand) } });
                var queriedVideoHearing = await _bookingService.SaveNewHearingAndPublish(createVideoHearingCommand, request.IsMultiDayHearing);
                _ivhLogger.TrackTrace(logSaveSuccess, SeverityLevel.Information, new Dictionary<string, string> { { logNewHearingId, createVideoHearingCommand.NewHearingId.ToString() } });
                

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
                _ivhLogger.TrackTrace(logRetrieveNewHearing, SeverityLevel.Information, logTrace);
                
                var response = HearingToDetailsResponseMapper.Map(queriedVideoHearing);
                const string logProcessFinished = "BookNewHearing Finished, returning response";
                _ivhLogger.TrackTrace(logProcessFinished, SeverityLevel.Information, new Dictionary<string, string> { { "response", JsonConvert.SerializeObject(response) } });

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
                    _ivhLogger.TrackError(ex, errorLog);
                }
                throw;
            }
        }

        /// <summary>
        /// Rebook an existing hearing with a booking status of Failed
        /// </summary>
        /// <param name="hearingId">Id of the hearing with a status of Failed</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/conferences")]
        [OpenApiOperation("RebookHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RebookHearing(Guid hearingId)
        {
            var hearing = await GetHearingAsync(hearingId);

            if (hearing == null)
            {
                return NotFound();
            }

            if (hearing.Status != BookingStatus.Failed)
            {
                ModelState.AddModelError(nameof(hearingId), $"Hearing must have a status of {nameof(BookingStatus.Failed)}");
                return ValidationProblem(ModelState);
            }

            await _bookingService.PublishNewHearing(hearing, false);

            return NoContent();
        }

        private IActionResult ModelStateErrorLogger(string key, string exception, string logErrorMessage, string errorValue, SeverityLevel severity)
        {
            ModelState.AddModelError(key, exception);
            if (errorValue == null)
            {
                _ivhLogger.TrackTrace(logErrorMessage, severity);
            }
            else
            {
                _ivhLogger.TrackTrace(logErrorMessage, severity, new Dictionary<string, string> { { key, errorValue } });
            }
            return ValidationProblem(ModelState);
        }

        /// <summary>
        /// Create a new hearing with the details of a given hearing on given dates
        /// </summary>
        /// <param name="hearingId">Original hearing to clone</param>
        /// <param name="request">List of dates to create a new hearing on</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/clone")]
        [OpenApiOperation("CloneHearing")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CloneHearing([FromRoute] Guid hearingId,
            [FromBody] CloneHearingRequest request)
        {
            var videoHearing = await GetHearingAsync(hearingId);
            if (videoHearing == null)
            {
                return NotFound();
            }

            var validationResult = await new CloneHearingRequestValidation().ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var datesValidationResult =
                new CloneHearingRequestValidation(videoHearing)
                    .ValidateDates(request);
            if (!datesValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(datesValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var orderedDates = request.Dates.OrderBy(x => x).ToList();
            var totalDays = orderedDates.Count + 1; // include original hearing
            var commands = orderedDates.Select((newDate, index) =>
            {
                var hearingDay = index + 2; // zero index including original hearing
                return CloneHearingToCommandMapper.CloneToCommand(videoHearing, newDate, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem, totalDays, hearingDay, request.ScheduledDuration);
            }).ToList();

            var existingCase = videoHearing.GetCases()[0];
            await _hearingService.UpdateHearingCaseName(hearingId, $"{existingCase.Name} Day {1} of {totalDays}");
            var hearingsList = new List<VideoHearing>();
            foreach (var command in commands)
            {
                // dbcontext is not thread safe. loop one at a time
                var hearing = await _bookingService.SaveNewHearingAndPublish(command, true);
                hearingsList.Add(hearing);
            }
            
            // publish multi day hearing notification event
            await _bookingService.PublishMultiDayHearing(videoHearing, totalDays);
            var response = hearingsList.Select(HearingToDetailsResponseMapper.Map).ToList();

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
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateHearingDetails(Guid hearingId, [FromBody] UpdateHearingRequest request)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }
            
            var result = new UpdateHearingRequestValidation().Validate(request);

            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            var venueId = request.HearingVenueName;
            var venue = await GetVenue(venueId);
            if (venue == null)
            {
                ModelState.AddModelError(nameof(request.HearingVenueName), "Hearing venue does not exist");
                return ValidationProblem(ModelState);
            }

            var cases = MapCase(request.Cases);

            // use existing video hearing values here when request properties are null
            request.AudioRecordingRequired ??= videoHearing.AudioRecordingRequired;
            request.HearingRoomName ??= videoHearing.HearingRoomName;
            request.OtherInformation ??= videoHearing.OtherInformation;

            var command = new UpdateHearingCommand(hearingId, request.ScheduledDateTime,
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, request.AudioRecordingRequired.Value);
        
            var updatedHearing = await _bookingService.UpdateHearingAndPublish(command, videoHearing);
            var response = HearingToDetailsResponseMapper.Map(updatedHearing);
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
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RemoveHearing(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound($"{hearingId} does not exist");
            }

            var command = new RemoveHearingCommand(hearingId);

            await _commandHandler.Handle(command);

            await _bookingService.PublishHearingCancelled(videoHearing);
            return NoContent();
        }

        /// <summary>
        /// Updates the status of a hearing once conference created, to Created or ConfirmedWithoutJudge if Judge not yet assigned
        /// For internal use only
        /// </summary>
        /// <param name="hearingId">Id of the hearing to update the status for</param>
        /// <returns>Success status</returns>
        [HttpPatch("{hearingId}")]
        [OpenApiOperation("UpdateBookingStatus")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [MapToApiVersion("1.0")]
        public Task<IActionResult> UpdateBookingStatus(Guid hearingId)
        {
            return UpdateStatus(hearingId, BookingStatus.Created);
        }
        
        /// <summary>
        /// Mark the booking as failed, for internal system use only
        /// </summary>
        /// <param name="hearingId">Id of the hearing to cancel the booking for</param>
        /// <returns>Success status</returns>
        [HttpPatch("{hearingId}/fail")]
        [OpenApiOperation("FailBooking")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(SerializableError),(int)HttpStatusCode.Conflict)]
        [MapToApiVersion("1.0")]
        public Task<IActionResult> FailBooking(Guid hearingId)
        {
            return UpdateStatus(hearingId, BookingStatus.Failed);
        }
        
        /// <summary>
        /// Cancels the booking
        /// </summary>
        /// <param name="hearingId">Id of the hearing to cancel the booking for</param>
        /// <param name="request">Cancel reason</param>
        /// <returns>Success status</returns>
        [HttpPatch("{hearingId}/cancel")]
        [OpenApiOperation("CancelBooking")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(SerializableError),(int)HttpStatusCode.Conflict)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CancelBooking(Guid hearingId, CancelBookingRequest request)
        {
            var result = new CancelBookingRequestValidation().Validate(request);
            if (result.IsValid)
                return await UpdateStatus(hearingId, BookingStatus.Cancelled, request.UpdatedBy, request.CancelReason);
            
            ModelState.AddFluentValidationErrors(result.Errors);
            return ValidationProblem(ModelState);
        }

        private async Task<IActionResult> UpdateStatus(Guid hearingId, BookingStatus status, string updatedBy = "System", string reason = null)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            if (videoHearing == null)
                return NotFound();
            try
            {
                await _bookingService.UpdateHearingStatus(videoHearing, status, updatedBy, reason);

                return NoContent();
            }
            catch (DomainRuleException exception)
            {
                exception.ValidationFailures.ForEach(x => ModelState.AddModelError(x.Name, x.Message));
                return Conflict(ModelState);
            }
        }
        
        /// <summary>
        /// Get all hearings by a given case type
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A cursor-based result of a list of matching hearings</returns>
        [HttpPost("types")]
        [OpenApiOperation("GetHearingsByTypes")]
        [ProducesResponseType(typeof(BookingsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BookingsResponse>> GetHearingsByTypes([FromBody] GetHearingRequest request)
        {
            request.FromDate ??= DateTime.UtcNow.Date;
            request.Types ??= new List<int>();

            if (!await ValidateCaseTypes(request.Types))
            {
                ModelState.AddModelError("Hearing types", "Invalid value for hearing types");
                return ValidationProblem(ModelState);
            }

            request.VenueIds ??= new List<int>();
            if (!await ValidateVenueIds(request.VenueIds))
            {
                ModelState.AddModelError("Venue ids", "Invalid value for venue ids");
                return ValidationProblem(ModelState);
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
                NoJudge = request.NoJudge,
                Unallocated = request.NoAllocated,
                CaseTypes = request.Types,
                SelectedUsers = request.Users
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

        private async Task<VideoHearing> GetHearingAsync(Guid hearingId)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            return await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        }

        private async Task<HearingVenue> GetVenue(string venueId)
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);
            return hearingVenues.SingleOrDefault(x => x.Name.Equals(venueId, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Search for hearings by case number. Search will apply fuzzy matching
        /// </summary>
        /// <param name="searchQuery">Search criteria</param>
        /// <returns>list of hearings matching search criteria</returns>
        [HttpGet("audiorecording/search")]
        [OpenApiOperation("SearchForHearings")]
        [ProducesResponseType(typeof(List<AudioRecordedHearingsBySearchResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
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
        /// Get booking status for a given hearing id
        /// </summary>
        /// <param name="hearingId">Id for a hearing</param>
        /// <returns>Booking status</returns>
        [HttpGet("{hearingId}/status")]
        [OpenApiOperation("GetBookingStatusById")]
        [ProducesResponseType(typeof(Contract.V1.Enums.BookingStatus), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetBookingStatusById(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }

            var query = new GetHearingShellByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingShellByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }

            return Ok((Contract.V1.Enums.BookingStatus)videoHearing.Status);
        }

        /// <summary>
        /// Return hearing details for todays hearings
        /// </summary>
        /// <returns>Booking status</returns>
        [HttpGet("today")]
        [OpenApiOperation("GetHearingsForToday")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingsForToday()
        {
            var videoHearings = await _queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(new GetHearingsForTodayQuery());
            if (!videoHearings.Any())
                return NotFound();

            return Ok(videoHearings.Select(HearingToDetailsResponseMapper.Map).ToList());
        }
        
        /// <summary>
        /// Return hearing details for todays hearings by venue
        /// </summary>
        /// <param name="venueNames">List of hearing venue names provided in payload</param>
        /// <returns>Booking status</returns>
        [HttpPost("today/venue")]
        [OpenApiOperation("GetHearingsForTodayByVenue")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingsForTodayByVenue([FromBody]IEnumerable<string> venueNames)
        {
            var videoHearings = await _queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(new GetHearingsForTodayQuery(venueNames));
            if (!videoHearings.Any())
                return NotFound();

            return Ok(videoHearings.Select(HearingToDetailsResponseMapper.Map).ToList());
        }

        private static string BuildCursorPageUrl(
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


            return pageUrl;
        }

        private async Task<bool> ValidateCaseTypes(List<int> filterCaseTypes)
        {
            if (!filterCaseTypes.Any())
            {
                return true;
            }

            var query = new GetAllCaseTypesQuery(includeDeleted:true);
            var validCaseTypes = (await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query))
                .Select(caseType => caseType.Id);

            return filterCaseTypes.TrueForAll(caseType => validCaseTypes.Contains(caseType));

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

            return filterVenueIds.TrueForAll(venueId => validVenueIds.Contains(venueId));
        }

        private static List<Case> MapCase(List<CaseRequest> caseRequestList)
        {
            var cases = caseRequestList ?? new List<CaseRequest>();
            return cases.Select(caseRequest => new Case(caseRequest.Number, caseRequest.Name)).ToList();
        }
    }
}
