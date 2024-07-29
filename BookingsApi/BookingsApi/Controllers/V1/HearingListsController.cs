using BookingsApi.Contract.V1.Requests;
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
    /// <param name="request">A filter, either CSO ids or unallocated</param>
    /// <returns>List of hearings that match the cso ids. If no ids provided an empty list is returned</returns>
    [HttpPost("today/csos")]
    [OpenApiOperation("GetHearingsForTodayByCsos")]
    [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetHearingsForTodayByCsos([FromBody] HearingsForTodayByAllocationRequest request)
    {
        if (request.CsoIds.Count == 0 && !request.Unallocated.HasValue)
        {
            ModelState.AddModelError(nameof(request), "Provide at least one filter type");
            return ValidationProblem(ModelState);
        }
            
        var videoHearings =
            await queryHandler.Handle<GetHearingsForTodayQueryAllocatedToQuery, List<VideoHearing>>(
                new GetHearingsForTodayQueryAllocatedToQuery(request.CsoIds, request.Unallocated));
        return Ok(videoHearings.Count == 0 ? [] : videoHearings.Select(HearingToDetailsResponseMapper.Map).ToList());
    }
}