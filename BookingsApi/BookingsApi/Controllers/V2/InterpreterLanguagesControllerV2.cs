using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;

namespace BookingsApi.Controllers.V2;

[Produces("application/json")]
[Route(template: "v{version:apiVersion}/InterpreterLanguages")]
[ApiVersion("2.0")]
[ApiController]
public class InterpreterLanguagesControllerV2(IQueryHandler queryHandler) : ControllerBase
{
    /// <summary>
    /// Get available interpreter languages
    /// </summary>
    /// <returns>List of languages, their codes and description</returns>
    [HttpGet]
    [OpenApiOperation("GetAvailableInterpreterLanguages")]
    [ProducesResponseType(typeof(List<InterpreterLanguagesResponse>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> GetAvailableInterpreterLanguages()
    {
        var query = new GetInterpreterLanguages();
        var languages = await queryHandler.Handle<GetInterpreterLanguages, List<InterpreterLanguage>>(query);
        var response = languages.Select(InterpreterLanguageToResponseMapperV2.MapInterpreterLanguageToResponse).ToList();

        return Ok(response);
    }
}