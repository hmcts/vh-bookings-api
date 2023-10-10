using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Controllers.V1;

[Produces("application/json")]
[Route(template:"v{version:apiVersion}/hearingroles")]
[Route("hearingroles")]
[ApiVersion("1.0")]
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
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetHearingRoles()
    {
        var query = new GetHearingRolesQuery();
        var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(query);

        var response = hearingRoles
            .Select(x => new HearingRoleResponseV2
            {
                Name = x.Name, 
                UserRole = x.UserRole.Name,
                Code = x.Code
            })
            .ToList();

        return Ok(response);
    }
}