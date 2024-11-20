using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Services;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;

namespace BookingsApi.Controllers.V2
{
    /// <summary>
    /// A suite of operations to manage bookings (V2)
    /// </summary>
    [Produces("application/json")]
    [Route(template: "v{version:apiVersion}/hearings")]
    [ApiController]
    [ApiVersion("2.0")]
    public class HearingsControllerV2(
        IQueryHandler queryHandler,
        IBookingService bookingService,
        ILogger<HearingsControllerV2> logger,
        IRandomGenerator randomGenerator,
        IUpdateHearingService updateHearingService,
        IEndpointService endpointService,
        IFeatureToggles featureToggles,
        IHearingService hearingService)
        : ControllerBase
    {
        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [OpenApiOperation("BookNewHearingWithCode")]
        [ProducesResponseType(typeof(HearingDetailsResponseV2), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> BookNewHearingWithCode(BookNewHearingRequestV2 request)
        {
            request.SanitizeRequest();
            if(!featureToggles.UseVodafoneToggle())
            {
                request.BookingSupplier = BookingSupplier.Kinly;
            }
            request.BookingSupplier ??=
                featureToggles.UseVodafoneToggle() ? BookingSupplier.Vodafone : BookingSupplier.Kinly;
            var result = await new BookNewHearingRequestInputValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            var hearingRoles =
                await queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            var caseType =
                await queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(
                    new GetCaseRolesForCaseServiceQuery(request.ServiceId));
            var hearingVenue = await GetHearingVenue(request.HearingVenueCode);

            var dataValidationResult =
                await new BookNewHearingRequestRefDataValidationV2(caseType, hearingVenue, hearingRoles)
                    .ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();

            var sipAddressStem = endpointService.GetSipAddressStem(request.BookingSupplier);
            var createVideoHearingCommand = BookNewHearingRequestV2ToCreateVideoHearingCommandMapper.Map(
                request, caseType, hearingVenue, cases, randomGenerator, sipAddressStem, hearingRoles);

            var queriedVideoHearing =
                await bookingService.SaveNewHearingAndPublish(createVideoHearingCommand, request.IsMultiDayHearing);

            var response = HearingToDetailsResponseV2Mapper.Map(queriedVideoHearing);
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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetHearingDetailsById(Guid hearingId)
        {
            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

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
        [OpenApiOperation("UpdateHearingDetailsV2")]
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
            var videoHearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var venue = await GetHearingVenue(request.HearingVenueCode);
            if (venue == null)
            {
                ModelState.AddModelError(nameof(request.HearingVenueCode),
                    $"Hearing venue code {request.HearingVenueCode} does not exist");
                logger.LogTrace("HearingVenueCode {HearingVenueCode} does not exist", request.HearingVenueCode);
                return ValidationProblem(ModelState);
            }

            var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();

            // use existing video hearing values here when request properties are null
            request.AudioRecordingRequired ??= videoHearing.AudioRecordingRequired;
            request.HearingRoomName ??= videoHearing.HearingRoomName;
            request.OtherInformation ??= videoHearing.OtherInformation;
            var scheduledDateTime = request.ScheduledDateTime.GetValueOrDefault(videoHearing.ScheduledDateTime);

            if (videoHearing.SourceId != null)
            {
                await bookingService.ValidateScheduleUpdateForHearingInGroup(videoHearing, scheduledDateTime);
            }

            var updatedHearing = await UpdateHearingDetails(hearingId, scheduledDateTime,
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
            var hearings = await queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(query);

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
        public async Task<IActionResult> UpdateHearingsInGroup(Guid groupId,
            [FromBody] UpdateHearingsInGroupRequestV2 request)
        {
            var inputValidationResult =
                await new UpdateHearingsInGroupRequestInputValidationV2().ValidateAsync(request);
            if (!inputValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(inputValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var getHearingsByGroupIdQuery = new GetHearingsByGroupIdQuery(groupId);
            var hearingsInGroup =
                await queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(getHearingsByGroupIdQuery);

            if (hearingsInGroup.Count == 0)
            {
                return NotFound();
            }

            var hearingRoles =
                await queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            var venues = await GetHearingVenues();

            var dataValidationResult =
                await new UpdateHearingsInGroupRequestRefDataValidationV2(hearingsInGroup, hearingRoles, venues)
                    .ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            foreach (var requestHearing in request.Hearings)
            {
                var hearing = hearingsInGroup.First(h => h.Id == requestHearing.HearingId);
                var venue = venues.Find(v => v.VenueCode == requestHearing.HearingVenueCode);
                var cases = hearing.GetCases()
                    .Select(x => new Case(requestHearing.CaseNumber, x.Name))
                    .ToList();

                await UpdateHearingDetails(hearing.Id, requestHearing.ScheduledDateTime,
                    requestHearing.ScheduledDuration, venue, requestHearing.HearingRoomName,
                    requestHearing.OtherInformation,
                    request.UpdatedBy, cases, requestHearing.AudioRecordingRequired, hearing);

                await updateHearingService.UpdateParticipantsV2(requestHearing.Participants, hearing, hearingRoles);

                hearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(
                    new GetHearingByIdQuery(requestHearing.HearingId));

                await updateHearingService.UpdateEndpointsV2(requestHearing.Endpoints, hearing);
                await updateHearingService.UpdateJudiciaryParticipantsV2(requestHearing.JudiciaryParticipants, hearing);
            }

            var hearings = request.Hearings.ToList();
            var totalDays = hearings.Count;
            var firstHearingId = hearings[0].HearingId;
            var firstHearing = await bookingService.GetHearingById(firstHearingId);
            var videoHearingUpdateDate = firstHearing.UpdatedDate.TrimSeconds();

            // publish multi day hearing notification event
            await bookingService.PublishEditMultiDayHearing(firstHearing, totalDays, videoHearingUpdateDate);

            return NoContent();
        }

        /// <summary>
        /// Create a new hearing with the details of a given hearing on given dates
        /// </summary>
        /// <param name="hearingId">Original hearing to clone</param>
        /// <param name="request">List of dates to create a new hearing on</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/clone")]
        [OpenApiOperation("CloneHearing")]
        [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> CloneHearing([FromRoute] Guid hearingId,
            [FromBody] CloneHearingRequestV2 request)
        {
            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
                return NotFound();

            var videoHearingUpdateDate = videoHearing.UpdatedDate.TrimSeconds();

            var validationResult = await new CloneHearingRequestValidationV2().ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var datesValidationResult =
                new CloneHearingRequestValidationV2(videoHearing)
                    .ValidateDates(request);
            if (!datesValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(datesValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var orderedDates = request.Dates.OrderBy(x => x).ToList();
            var totalDays = orderedDates.Count + 1; // include original hearing
            var sipAddressStem = endpointService.GetSipAddressStem((BookingSupplier?)videoHearing.ConferenceSupplier);
            var commands = orderedDates.Select((newDate, index) =>
            {
                var hearingDay = index + 2; // zero index including original hearing
                return CloneHearingToCommandMapper.CloneToCommand(videoHearing, newDate, randomGenerator,
                    sipAddressStem, totalDays, hearingDay, request.ScheduledDuration);
            }).ToList();

            var existingCase = videoHearing.GetCases()[0];
            await hearingService.RenameHearingForMultiDayBooking(hearingId,
                $"{existingCase.Name} Day {1} of {totalDays}");
            var hearingsList = new List<VideoHearing>();
            foreach (var command in commands)
            {
                // dbcontext is not thread safe. loop one at a time
                var hearing = await bookingService.SaveNewHearing(command);
                hearingsList.Add(hearing);
            }

            // publish multi day hearing notification event
            await bookingService.PublishMultiDayHearing(videoHearing, totalDays, videoHearingUpdateDate);
            var response = hearingsList.Select(HearingToDetailsResponseV2Mapper.Map).ToList();

            return Ok(response);
        }

        private async Task<HearingVenue> GetHearingVenue(string venueCode)
        {
            var hearingVenues =
                await queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(new GetHearingVenuesQuery());
            var hearingVenue = hearingVenues.SingleOrDefault(x =>
                string.Equals(x.VenueCode, venueCode, StringComparison.CurrentCultureIgnoreCase));
            return hearingVenue;
        }

        private async Task<List<HearingVenue>> GetHearingVenues()
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);

            return hearingVenues;
        }

        private async Task<Hearing> UpdateHearingDetails(Guid hearingId, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue venue, string hearingRoomName, string otherInformation,
            string updatedBy, List<Case> cases, bool audioRecordingRequired, VideoHearing originalHearing)
        {
            var command = new UpdateHearingCommand(hearingId, scheduledDateTime,
                scheduledDuration, venue, hearingRoomName, otherInformation,
                updatedBy, cases, audioRecordingRequired);

            var updatedHearing = await bookingService.UpdateHearingAndPublish(command, originalHearing);
            return updatedHearing;
        }
    }
}