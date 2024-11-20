using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;

namespace BookingsApi.Controllers.V2;

/// <summary>
/// A suite of operations to get hearing lists
/// </summary>
[Produces("application/json")]
[Route(template: "v{version:apiVersion}/hearings")]
[ApiVersion("2.0")]
[ApiController]
public class HearingListsControllerV2(IQueryHandler queryHandler) : ControllerBase
{
    /// <summary>
    /// Return hearing details for todays hearings
    /// </summary>
    /// <returns>Booking status</returns>
    [HttpGet("today")]
    [OpenApiOperation("GetHearingsForTodayV2")]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingsForToday()
    {
        var videoHearings =
            await queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(new GetHearingsForTodayQuery());

        return Ok(videoHearings.Count == 0 ? [] : videoHearings.Select(HearingToDetailsResponseV2Mapper.Map).ToList());
    }

    /// <summary>
    /// Return hearing details for todays hearings by venue
    /// </summary>
    /// <param name="venueNames">List of hearing venue names provided in payload</param>
    /// <returns>Booking status</returns>
    [HttpPost("today/venue")]
    [OpenApiOperation("GetHearingsForTodayByVenueV2")]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingsForTodayByVenue([FromBody] IEnumerable<string> venueNames)
    {
        var videoHearings =
            await queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(
                new GetHearingsForTodayQuery(venueNames));

        return Ok(videoHearings.Count == 0 ? [] : videoHearings.Select(HearingToDetailsResponseV2Mapper.Map).ToList());
    }

    /// <summary>
    /// Get list of all confirmed hearings for a given username for today
    /// </summary>
    /// <param name="username">username of person to search against</param>
    /// <returns>Hearing details</returns>
    [HttpGet("today/username")]
    [OpenApiOperation("GetConfirmedHearingsByUsernameForTodayV2")]
    [ProducesResponseType(typeof(List<ConfirmedHearingsTodayResponseV2>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetConfirmedHearingsByUsernameForToday([FromQuery] string username)
    {
        var query = new GetConfirmedHearingsByUsernameForTodayQuery(username);
        var hearings =
            await queryHandler.Handle<GetConfirmedHearingsByUsernameForTodayQuery, List<VideoHearing>>(query);

        var response = hearings.Select(ConfirmedHearingsTodayResponseMapperV2.Map).ToList();
        return Ok(response);
    }


    /// <summary>
    /// Retrieve a list of hearing allocated to a given list of CSOs
    /// </summary>
    /// <param name="request">A filter, either CSO ids or unallocated</param>
    /// <returns>List of hearings that match the cso ids. If no ids provided an empty list is returned</returns>
    [HttpPost("today/csos")]
    [OpenApiOperation("GetHearingsForTodayByCsosV2")]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingsForTodayByCsosV2(
        [FromBody] HearingsForTodayByAllocationRequestV2 request)
    {
        if (request.CsoIds.Count == 0 && !request.Unallocated.HasValue)
        {
            ModelState.AddModelError(nameof(request), "Provide at least one filter type");
            return ValidationProblem(ModelState);
        }

        var videoHearings =
            await queryHandler.Handle<GetHearingsForTodayQueryAllocatedToQuery, List<VideoHearing>>(
                new GetHearingsForTodayQueryAllocatedToQuery(request.CsoIds, request.Unallocated));
        return Ok(videoHearings.Count == 0 ? [] : videoHearings.Select(HearingToDetailsResponseV2Mapper.Map).ToList());
    }
    
    /// <summary>
    /// Get list of all hearings for notification between next 48 to 72 hrs. 
    /// </summary>
    /// <returns>Hearing details</returns>
    [HttpGet("notifications/gethearings")]
    [OpenApiOperation("GetHearingsForNotification")]
    [ProducesResponseType(typeof(List<HearingNotificationResponseV2>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingsForNotificationAsync()
    {
        var query = new GetHearingsForNotificationsQuery();

        var hearings = await queryHandler.Handle<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>(query);

        var response = hearings
            .Select(h => new HearingNotificationResponseV2
            {
                Hearing = HearingToDetailsResponseV2Mapper.Map(h.Hearing),
                TotalDays = h.TotalDays,
                SourceHearing = h.SourceHearing != null ? HearingToDetailsResponseV2Mapper.Map(h.SourceHearing) : null
            })
            .ToList();

        return Ok(response);
    }
}