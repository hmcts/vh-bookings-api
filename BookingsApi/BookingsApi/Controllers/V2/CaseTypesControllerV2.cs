using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Controllers.V2;

[Produces("application/json")]
[Route(template:"v{version:apiVersion}/casetypes")]
[ApiVersion("2.0")]
[ApiController]
public class CaseTypesController : Controller
{
    private readonly IQueryHandler _queryHandler;
    public CaseTypesController(IQueryHandler queryHandler)
    {
        _queryHandler = queryHandler;
    }
    
    /// <summary>
    /// Get case roles for a case service type
    /// </summary>
    /// <param name="serviceId"></param>
    /// <returns>Available case roles for given case type</returns>
    [HttpGet("{serviceId}/caseroles")]
    [OpenApiOperation("GetCaseRolesForCaseService")]
    [ProducesResponseType(typeof(List<CaseRoleResponseV2>), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetCaseRolesForCaseService(string serviceId)
    {
        var query = new GetCaseRolesForCaseServiceQuery(serviceId);
        var caseType = await _queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(query);
        if (caseType == null)
            return NotFound();

        var response = caseType.CaseRoles
            .Select(x => new CaseRoleResponseV2{ Name = x.Name })
            .ToList();

        return Ok(response);
    }
    
    
    /// <summary>
    /// Get hearing roles for a case role of a case service type
    /// </summary>
    /// <param name="serviceId">Hearing case service ID</param>
    /// <param name="caseRoleName">Hearing case role</param>
    /// <returns>Available hearing roles for given case role</returns>
    [HttpGet("{serviceId}/caseroles/{caseRoleName}/hearingroles")]
    [OpenApiOperation("GetHearingRolesForCaseRoleV2")]
    [ProducesResponseType(typeof(List<HearingRoleResponseV2>), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetHearingRolesForCaseRoleV2(string serviceId, string caseRoleName)
    {            
        var query = new GetCaseRolesForCaseServiceQuery(serviceId);
        var caseType = await _queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(query);
        if (caseType == null)
            return NotFound();

        var caseRole = caseType.CaseRoles.SingleOrDefault(x => x.Name == caseRoleName);
        if (caseRole == null)
            return NotFound();

        var response = caseRole.HearingRoles
            .Where(hr => hr.Live)
            .Select(x => new HearingRoleResponseV2 { Name = x.Name, UserRole = x.UserRole?.Name.ToString() })
            .ToList();

        return Ok(response);
    }
}