using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;
using FluentValidation.Results;

namespace BookingsApi.Controllers.V2
{
    [Produces("application/json")]
    [Route(template:"v{version:apiVersion}/hearings")]
    [ApiController]
    [ApiVersion("2.0")]
    public class HearingsControllerV2 : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IBookingService _bookingService;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly ILogger<HearingsControllerV2> _logger;
        private readonly IUpdateHearingService _updateHearingService;

        public HearingsControllerV2(IQueryHandler queryHandler, IBookingService bookingService,
            ILogger<HearingsControllerV2> logger, IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfigurationOption, IUpdateHearingService updateHearingService)
        {
            _queryHandler = queryHandler;
            _bookingService = bookingService;
            _logger = logger;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfigurationOption.Value;
            _updateHearingService = updateHearingService;
        }

        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [OpenApiOperation("BookNewHearingWithCode")]
        [ProducesResponseType(typeof(HearingDetailsResponseV2), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> BookNewHearingWithCode(BookNewHearingRequestV2 request)
        {
            request.SanitizeRequest();
            var result = await new BookNewHearingRequestInputValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }
            
            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(new GetCaseRolesForCaseServiceQuery(request.ServiceId));
            var hearingVenue = await GetHearingVenue(request.HearingVenueCode);
            
            var dataValidationResult = await new BookNewHearingRequestRefDataValidationV2(caseType, hearingVenue, hearingRoles).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
            
            var createVideoHearingCommand = BookNewHearingRequestV2ToCreateVideoHearingCommandMapper.Map(
                request, caseType, hearingVenue, cases, _randomGenerator, _kinlyConfiguration.SipAddressStem, hearingRoles);

            var queriedVideoHearing = await _bookingService.SaveNewHearingAndPublish(createVideoHearingCommand, request.IsMultiDayHearing);
            
            var response = HearingToDetailsResponseV2Mapper.Map(queriedVideoHearing);
            return CreatedAtAction(nameof(GetHearingDetailsById), new {hearingId = response.Id}, response);
        }

        /// <summary>
        /// Get details for a given hearing
        /// </summary>
        /// <param name="hearingId">Id for a hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{hearingId}")]
        [OpenApiOperation("GetHearingDetailsByIdV2")]
        [ProducesResponseType(typeof(HearingDetailsResponseV2), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetHearingDetailsById(Guid hearingId)
        {
            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
                return NotFound();

            var response = HearingToDetailsResponseV2Mapper.Map(videoHearing);
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

            var command = new UpdateHearingCommand(hearingId, request.ScheduledDateTime.GetValueOrDefault(videoHearing.ScheduledDateTime),
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, request.AudioRecordingRequired.Value);

            var updatedHearing = await _bookingService.UpdateHearingAndPublish(command, videoHearing);
            var response = HearingToDetailsResponseV2Mapper.Map(updatedHearing);
            return Ok(response);
        }

        /// <summary>
        /// Get list of all hearings in a group
        /// </summary>
        /// <param name="groupId">the group id of the single day or multi day hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{groupId}/hearings")]
        [OpenApiOperation("GetHearingsByGroupIdV2")]
        [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetHearingsByGroupId(Guid groupId)
        {
            var query = new GetHearingsByGroupIdQuery(groupId);
            var hearings = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(query);

            var response = hearings.Select(HearingToDetailsResponseV2Mapper.Map).ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Update hearings in a multi day group
        /// </summary>
        /// <param name="groupId">The group id of the multi day hearing</param>
        /// <param name="request">List of hearings to update</param>
        /// <returns>No content</returns>
        [HttpPatch("{groupId}/hearings")]
        [OpenApiOperation("UpdateHearingsInGroupV2")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateHearingsInGroup(Guid groupId, [FromBody] UpdateHearingsInGroupRequestV2 request)
        {
            if (request.Hearings == null || !request.Hearings.Any())
            {
                ModelState.AddModelError("hearings", "Please provide at least one hearing");
                return ValidationProblem(ModelState);
            }
            
            var duplicateHearingIds = request.Hearings.GroupBy(x => x.HearingId)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
            foreach (var duplicateHearingId in duplicateHearingIds)
            {
                var index = request.Hearings.FindIndex(h => h.HearingId == duplicateHearingId);
                
                ModelState.AddModelError($"hearings[{index}].HearingId", 
                    $"Duplicate hearing id {duplicateHearingId}");
            }

            if (duplicateHearingIds.Any())
            {
                return ValidationProblem(ModelState);
            }
            
            var getHearingsByGroupIdQuery = new GetHearingsByGroupIdQuery(groupId);
            var hearings = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(getHearingsByGroupIdQuery);

            if (!hearings.Any())
            {
                return NotFound();
            }
            
            var hearingsNotInGroup = request.Hearings.
                Where(requestHearing => !hearings.Exists(h => h.Id == requestHearing.HearingId))
                .ToList();

            foreach (var hearing in hearingsNotInGroup)
            {
                var index = request.Hearings.FindIndex(h => h.HearingId == hearing.HearingId);
                
                ModelState.AddModelError($"hearings[{index}].HearingId",
                    $"Hearing {hearing.HearingId} does not belong to group {groupId}");
            }

            if (hearingsNotInGroup.Any())
            {
                return ValidationProblem(ModelState);
            }
            
            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());

            foreach (var requestHearing in request.Hearings)
            {
                var participantsValidationResult = await ValidateUpdateParticipantsV2(requestHearing.Participants, hearingRoles);
                if (!participantsValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(participantsValidationResult.Errors);
                    return ValidationProblem(ModelState);
                }

                var endpointsValidationResult = await ValidateUpdateEndpointsV2(requestHearing.Endpoints);
                if (!endpointsValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(endpointsValidationResult.Errors);
                    return ValidationProblem(ModelState);
                }

                var judiciaryParticipantsValidationResult = await ValidateUpdateJudiciaryParticipantsV2(requestHearing.JudiciaryParticipants);
                if (!judiciaryParticipantsValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(judiciaryParticipantsValidationResult.Errors);
                    return ValidationProblem(ModelState);
                }
            }

            foreach (var requestHearing in request.Hearings)
            {
                var hearing = hearings.First(h => h.Id == requestHearing.HearingId);

                // TODO make sure we're passing in an updated hearing object here
                await _updateHearingService.UpdateParticipantsV2(requestHearing.Participants, hearing, hearingRoles);
                await _updateHearingService.UpdateEndpointsV2(requestHearing.Endpoints, hearing);
                await _updateHearingService.UpdateJudiciaryParticipantsV2(requestHearing.JudiciaryParticipants, hearing.Id);
            }

            return NoContent();
        }
        
        private static async Task<ValidationResult> ValidateUpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, List<HearingRole> hearingRoles)
        {
            var result = await new UpdateHearingParticipantsRequestInputValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                return result;
            }

            var dataValidationResult = await new UpdateHearingParticipantsRequestRefDataValidationV2(hearingRoles).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                return dataValidationResult;
            }

            return result;
        }

        private static async Task<ValidationResult> ValidateUpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request)
        {
            foreach (var newEndpoint in request.NewEndpoints)
            {
                var result = await new EndpointRequestValidationV2().ValidateAsync(newEndpoint);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            foreach (var existingEndpoint in request.ExistingEndpoints)
            {
                var result = await new EndpointRequestValidationV2().ValidateAsync(existingEndpoint);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            return new ValidationResult();
        }

        private static async Task<ValidationResult> ValidateUpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request)
        {
            foreach (var newJudiciaryParticipant in request.NewJudiciaryParticipants)
            {
                var result = await new JudiciaryParticipantRequestValidationV2().ValidateAsync(newJudiciaryParticipant);
                if (!result.IsValid)
                {
                    return result;
                }
            }

            foreach (var existingJudiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var existingJudiciaryParticipantValidationResult = await new UpdateJudiciaryParticipantRequestValidationV2().ValidateAsync(existingJudiciaryParticipant);
                if (!existingJudiciaryParticipantValidationResult.IsValid)
                {
                    return existingJudiciaryParticipantValidationResult;
                }
            }

            return new ValidationResult();
        }
        
        private async Task<HearingVenue> GetHearingVenue(string venueCode)
        {
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(new GetHearingVenuesQuery());
            var hearingVenue = hearingVenues.SingleOrDefault(x =>
                string.Equals(x.VenueCode, venueCode, StringComparison.CurrentCultureIgnoreCase));
            return hearingVenue;
        }
    }
}