using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Controllers.V2;

[Produces("application/json")]
[Route(template:"v{version:apiVersion}/casetypes")]
[ApiVersion("2.0")]
[ApiController]
public class CaseTypesController(IQueryHandler queryHandler) : ControllerBase
{
    /// <summary>
    ///     Get available case types
    /// </summary>
    /// <returns>A list of available case types</returns>
    [HttpGet]
    [OpenApiOperation("GetCaseTypesV2")]
    [ProducesResponseType(typeof(List<CaseTypeResponseV2>), (int) HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetCaseTypes([FromQuery]bool includeDeleted = false)
    {
        var query = new GetAllCaseTypesQuery(includeDeleted);
        var caseTypes = await queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query);
        var response = caseTypes.Select(caseType => new CaseTypeResponseV2
            {
                Id = caseType.Id,
                Name = caseType.Name,
                ServiceId = caseType.ServiceId,
                IsAudioRecordingAllowed = caseType.IsAudioRecordingAllowed
            }
        );
        return Ok(response);
    }
}