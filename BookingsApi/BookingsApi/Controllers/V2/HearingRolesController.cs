using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Controllers.V2;

[Produces("application/json")]
[Route(template:"v{version:apiVersion}/hearingroles")]
[ApiVersion("2.0")]
[ApiController]
public class HearingRolesController : ControllerBase
{
    private readonly IQueryHandler _queryHandler;
    public HearingRolesController(IQueryHandler queryHandler)
    {
        _queryHandler = queryHandler;
    }
    
    /// <summary>
    /// Get all standard hearing roles
    /// </summary>=
    /// <returns>Available case roles for given case type</returns>
    [HttpGet]
    [OpenApiOperation("GetHearingRoles")]
    [ProducesResponseType(typeof(List<HearingRoleResponseV2>), (int) HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingRoles()
    {
        var query = new GetHearingRolesQuery();
        var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(query);

        var response = hearingRoles
            .Select(x => new HearingRoleResponseV2{ Name = x.Name, UserRole = x.UserRole.Name})
            .ToList();

        return Ok(response);
    }
}