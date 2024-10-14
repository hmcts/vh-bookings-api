using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using Microsoft.AspNetCore.Authorization;

namespace BookingsApi.Controllers.V1;

[Produces("application/json")]
[Route("InterpreterLanguages")]
[ApiVersion("1.0")]
[ApiController]
public class InterpreterLanguagesController(IQueryHandler queryHandler) : ControllerBase
{
    private readonly IQueryHandler _queryHandler = queryHandler;
    
    [HttpGet]
    [OpenApiOperation("GetAvailableInterpreterLanguages")]
    [ProducesResponseType(typeof(List<InterpreterLanguagesResponse>), (int)HttpStatusCode.OK)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAvailableInterpreterLanguages()
    {
        var query = new GetInterpreterLanguages();
        var languages = await _queryHandler.Handle<GetInterpreterLanguages, List<InterpreterLanguage>>(query);
        var response = languages.Select(InterpreterLanguageToResponseMapper.MapInterpreterLanguageToResponse).ToList();

        return Ok(response);
    }
}