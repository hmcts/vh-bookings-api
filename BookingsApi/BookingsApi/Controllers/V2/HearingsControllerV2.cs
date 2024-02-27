using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;

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

            var updatedHearing = await UpdateHearingDetails(hearingId, request.ScheduledDateTime.GetValueOrDefault(videoHearing.ScheduledDateTime),
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, request.AudioRecordingRequired.Value, videoHearing);
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
            var inputValidationResult = await new UpdateHearingsInGroupRequestInputValidationV2().ValidateAsync(request);
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

            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            
            var dataValidationResult = await new UpdateHearingsInGroupRequestRefDataValidationV2(hearingsInGroup, hearingRoles).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var venues = await GetHearingVenues();
            
            foreach (var requestHearing in request.Hearings)
            {
                var hearing = hearingsInGroup.First(h => h.Id == requestHearing.HearingId);
                var venue = venues.First(v => v.Id == hearing.HearingVenueId);
                var cases = hearing.GetCases().ToList();

                await UpdateHearingDetails(hearing.Id, hearing.ScheduledDateTime, 
                    hearing.ScheduledDuration, venue, hearing.HearingRoomName, hearing.OtherInformation, 
                    request.UpdatedBy, cases, hearing.AudioRecordingRequired, hearing);
                
                await _updateHearingService.UpdateParticipantsV2(requestHearing.Participants, hearing, hearingRoles);
                
                hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(requestHearing.HearingId));
                
                await _updateHearingService.UpdateEndpointsV2(requestHearing.Endpoints, hearing);
                await _updateHearingService.UpdateJudiciaryParticipantsV2(requestHearing.JudiciaryParticipants, hearing);
            }
            
            var hearings = request.Hearings.ToList();
            var totalDays = hearings.Count;
            var nextFirstHearingId = hearings[0].HearingId;
            
            var nextFirstHearing = await _bookingService.GetHearingById(nextFirstHearingId);
            
            if (nextFirstHearing == null)
                return NotFound();
            
            // publish multi day hearing notification event
            await _bookingService.PublishMultiDayHearing(nextFirstHearing, totalDays);

            return NoContent();
        }

        private async Task<HearingVenue> GetHearingVenue(string venueCode)
        {
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(new GetHearingVenuesQuery());
            var hearingVenue = hearingVenues.SingleOrDefault(x =>
                string.Equals(x.VenueCode, venueCode, StringComparison.CurrentCultureIgnoreCase));
            return hearingVenue;
        }
        
        private async Task<List<HearingVenue>> GetHearingVenues()
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);

            return hearingVenues;
        }
        
        private async Task<Hearing> UpdateHearingDetails(Guid hearingId, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue venue, string hearingRoomName, string otherInformation,
            string updatedBy, List<Case> cases, bool audioRecordingRequired, VideoHearing originalHearing)
        {
            var command = new UpdateHearingCommand(hearingId, scheduledDateTime,
                scheduledDuration, venue, hearingRoomName, otherInformation,
                updatedBy, cases, audioRecordingRequired);
            
            var updatedHearing = await _bookingService.UpdateHearingAndPublish(command, originalHearing);
            return updatedHearing;
        }
    }
}