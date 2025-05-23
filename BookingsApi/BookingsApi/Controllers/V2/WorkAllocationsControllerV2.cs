
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;

namespace BookingsApi.Controllers.V2;

[Produces("application/json")]
[Route(template: "v{version:apiVersion}/hearings")]
[ApiVersion("2.0")]
[ApiController]
public class WorkAllocationsControllerV2(IQueryHandler queryHandler) : ControllerBase
{
    /// <summary>
    /// Get all the unallocated hearings
    /// </summary>
    /// <returns>unallocated hearings</returns>
    [HttpGet("unallocated")]
    [OpenApiOperation("GetUnallocatedHearingsV2")]
    [ProducesResponseType(typeof(List<HearingDetailsResponseV2>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetUnallocatedHearingsV2()
    {
        var today = DateTime.UtcNow; //provide a range (from today 1 year) for unallocated hearings rather than return all past and present.
        var query = new GetAllocationHearingsBySearchQuery(isUnallocated: true, fromDate: today, toDate: today.AddYears(1), excludeDurationsThatSpanMultipleDays: true);
        var results = await queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);

        var response = results.Select(HearingToDetailsResponseV2Mapper.Map).ToList();
        return Ok(response);
    }

}