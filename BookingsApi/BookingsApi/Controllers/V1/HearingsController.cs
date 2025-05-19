using System.Diagnostics.CodeAnalysis;
using BookingsApi.Contract.V1.Queries;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1;

[Produces("application/json")]
[Route("hearings")]
[ApiVersion("1.0")]
[ApiController]
public class HearingsController(
    IQueryHandler queryHandler,
    ICommandHandler commandHandler,
    IBookingService bookingService)
    : ControllerBase
{
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
        await commandHandler.Handle(new AnonymiseCaseAndParticipantCommand { HearingIds = hearingIds });
        return Ok();
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
    public async Task<IActionResult> CancelHearingsInGroup(Guid groupId,
        [FromBody] CancelHearingsInGroupRequest request)
    {
        var inputValidationResult = await new CancelHearingsInGroupRequestInputValidation().ValidateAsync(request);
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

        var dataValidationResult =
            await new CancelHearingsInGroupRequestRefDataValidation(hearingsInGroup).ValidateAsync(request);
        if (!dataValidationResult.IsValid)
        {
            ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
            return ValidationProblem(ModelState);
        }

        var requestHearings = hearingsInGroup.Where(h => request.HearingIds.Contains(h.Id)).ToList();
        var hearingDataValidationResult =
            await new CancelHearingsInGroupRequestHearingRefDataValidation(requestHearings).ValidateAsync(request);
        if (!hearingDataValidationResult.IsValid)
        {
            ModelState.AddFluentValidationErrors(hearingDataValidationResult.Errors);
            return ValidationProblem(ModelState);
        }

        foreach (var hearingId in request.HearingIds)
        {
            var hearing = hearingsInGroup.Find(h => h.Id == hearingId);

            await bookingService.UpdateHearingStatus(hearing, BookingStatus.Cancelled, request.UpdatedBy,
                request.CancelReason);
        }

        return NoContent();
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
            ModelState.AddModelError(nameof(hearingId),
                $"Hearing must have a status of {nameof(BookingStatus.Failed)}");
            return ValidationProblem(ModelState);
        }

        await bookingService.PublishNewHearing(hearing, false);

        return NoContent();
    }

    /// <summary>
    /// Remove an existing hearing
    /// For internal use only
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
        var videoHearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

        if (videoHearing == null)
        {
            return NotFound($"{hearingId} does not exist");
        }

        var command = new RemoveHearingCommand(hearingId);

        await commandHandler.Handle(command);

        await bookingService.PublishHearingCancelled(videoHearing);
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
    [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.Conflict)]
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
    [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.Conflict)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> CancelBooking(Guid hearingId, CancelBookingRequest request)
    {
        var result = await new CancelBookingRequestValidation().ValidateAsync(request);
        if (result.IsValid)
            return await UpdateStatus(hearingId, BookingStatus.Cancelled, request.UpdatedBy, request.CancelReason);

        ModelState.AddFluentValidationErrors(result.Errors);
        return ValidationProblem(ModelState);
    }

    private async Task<IActionResult> UpdateStatus(Guid hearingId, BookingStatus status, string updatedBy = "System",
        string reason = null)
    {
        if (hearingId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
            return ValidationProblem(ModelState);
        }

        var videoHearing =
            await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
        if (videoHearing == null)
            return NotFound();
        try
        {
            await bookingService.UpdateHearingStatus(videoHearing, status, updatedBy, reason);

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
        var result =
            await queryHandler.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(query);

        var mapper = new VideoHearingsToBookingsResponseMapper();

        var response = new BookingsResponse
        {
            PrevPageUrl = BuildCursorPageUrl(request.Cursor, request.Limit, request.Types, request.CaseNumber,
                request.VenueIds, request.LastName),
            NextPageUrl = BuildCursorPageUrl(result.NextCursor, request.Limit, request.Types, request.CaseNumber,
                request.VenueIds, request.LastName),
            NextCursor = result.NextCursor,
            Limit = request.Limit,
            Hearings = mapper.MapHearingResponses(result)
        };

        return Ok(response);
    }

    private async Task<VideoHearing> GetHearingAsync(Guid hearingId)
    {
        var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
        return await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
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
    [ExcludeFromCodeCoverage(Justification = "Deprecated feature but kept for internal testing")]
    public async Task<IActionResult> SearchForHearingsAsync([FromQuery] SearchForHearingsQuery searchQuery)
    {
        var caseNumber = WebUtility.UrlDecode(searchQuery.CaseNumber);

        var query = new GetAudioRecordedHearingsBySearchQuery(caseNumber, searchQuery.Date);
        var hearings = await queryHandler.Handle<GetAudioRecordedHearingsBySearchQuery, List<VideoHearing>>(query);

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
        var videoHearing = await queryHandler.Handle<GetHearingShellByIdQuery, VideoHearing>(query);

        if (videoHearing == null)
        {
            return NotFound();
        }

        return Ok((Contract.V1.Enums.BookingStatus)videoHearing.Status);
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
        if (caseTypes.Count != 0)
        {
            types = string.Join("&types=", caseTypes);
        }

        var pageUrl = $"{resourceUrl}?types={types}&cursor={cursor}&limit={limit}";

        if (!string.IsNullOrWhiteSpace(caseNumber))
        {
            pageUrl += $"&caseNumber={caseNumber}";
        }

        var venueIds = string.Empty;
        if (hearingVenueIds != null && hearingVenueIds.Count != 0)
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
        if (filterCaseTypes.Count == 0)
        {
            return true;
        }

        var query = new GetAllCaseTypesQuery(includeDeleted: true);
        var validCaseTypes = (await queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query))
            .Select(caseType => caseType.Id);

        return filterCaseTypes.TrueForAll(caseType => validCaseTypes.Contains(caseType));

    }

    private async Task<bool> ValidateVenueIds(List<int> filterVenueIds)
    {
        if (filterVenueIds.Count == 0)
        {
            return true;
        }

        var query = new GetHearingVenuesQuery();
        var validVenueIds = (await queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query))
            .Select(venue => venue.Id);

        return filterVenueIds.TrueForAll(venueId => validVenueIds.Contains(venueId));
    }
}