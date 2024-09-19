using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;

namespace BookingsApi.Controllers.V2;

/// <summary>
/// A suite of operations to get hearing lists
/// </summary>
[Produces("application/json")]
[Route(template:"v{version:apiVersion}/hearings")]
[ApiVersion("2.0")]
[ApiController]
public class HearingListsControllerV2(IQueryHandler queryHandler) : ControllerBase
{
    /// <summary>
    /// Retrieve a list of hearing allocated to a given list of CSOs
    /// </summary>
    /// <param name="request">A filter, either CSO ids or unallocated</param>
    /// <returns>List of hearings that match the cso ids. If no ids provided an empty list is returned</returns>
    [HttpPost("today/csos")]
    [OpenApiOperation("GetHearingsForTodayByCsosV2")]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingsForTodayByCsosV2([FromBody] HearingsForTodayByAllocationRequestV2 request)
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
}