using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Services;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;

namespace BookingsApi.Controllers.V2;

/// <summary>
/// Multi-day hearings controller
/// </summary>
[Consumes("application/json")]
[Produces("application/json")]
[ApiController]
public class MultiDaysControllerV2(
    IQueryHandler queryHandler,
    IBookingService bookingService,
    IRandomGenerator randomGenerator,
    IEndpointService endpointService,
    IHearingService hearingService)
    : ControllerBase
{
    /// <summary>
    /// Create a new hearing with the details of a given hearing on given dates
    /// </summary>
    /// <param name="hearingId">Original hearing to clone</param>
    /// <param name="request">List of dates to create a new hearing on</param>
    /// <returns></returns>
    [HttpPost("v2/hearings/{hearingId}/clone")]
    [OpenApiOperation("CloneHearing")]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CloneHearingAsync([FromRoute] Guid hearingId,
        [FromBody] CloneHearingRequestV2 request)
    {
        return await CloneHearing(hearingId, request);
    }

    /// <summary>
    /// Create a new hearing with the details of a given hearing on given dates. Used by S&amp;L until they migrate to v2 route
    /// </summary>
    /// <param name="hearingId">Original hearing to clone</param>
    /// <param name="request">List of dates to create a new hearing on</param>
    /// <returns></returns>
    [HttpPost("hearings/{hearingId}/clone")]
    [OpenApiOperation("CloneHearing")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CloneHearingForSAndLAsync([FromRoute] Guid hearingId,
        [FromBody] CloneHearingRequestV2 request)
    {
        return await CloneHearing(hearingId, request);
    }

    private async Task<IActionResult> CloneHearing(Guid hearingId, CloneHearingRequestV2 request)
    {
        var query = new GetHearingByIdQuery(hearingId);
        var videoHearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

        if (videoHearing == null)
            return NotFound();

        var videoHearingUpdateDate = videoHearing.UpdatedDate.Value.TrimSeconds();

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
}