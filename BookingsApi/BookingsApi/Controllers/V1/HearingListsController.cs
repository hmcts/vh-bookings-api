using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Controllers.V1;

[Produces("application/json")]
[Route("hearings")]
[ApiVersion("1.0")]
[ApiController]
public class HearingListsController(IQueryHandler queryHandler) : ControllerBase
{
    /// <summary>
    /// Retrieve a list of hearing allocated to a given list of CSOs
    /// </summary>
    /// <param name="csoIds">List of CSO ids</param>
    /// <returns>List of hearings that match the cso ids. If no ids provided an empty list is returned</returns>
    [HttpPost("today/csos")]
    [OpenApiOperation("GetHearingsForTodayByCsos")]
    [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetHearingsForTodayByCsos([FromBody] List<Guid> csoIds)
    {
        if (csoIds.Count == 0)
            return Ok(new List<HearingDetailsResponse>());
        var videoHearings =
            await queryHandler.Handle<GetHearingsForTodayQueryAllocatedToQuery, List<VideoHearing>>(
                new GetHearingsForTodayQueryAllocatedToQuery(csoIds));
        return Ok(videoHearings.Count == 0 ? [] : videoHearings.Select(HearingToDetailsResponseMapper.Map).ToList());
    }
}