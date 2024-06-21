using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1;

[Produces("application/json")]
[Route("InterpreterLanguages")]
[ApiVersion("1.0")]
[ApiController]
public class InterpreterLanguagesController(IQueryHandler queryHandler, ICommandHandler commandHandler) : ControllerBase
{
    [HttpGet]
    [OpenApiOperation("GetAvailableInterpreterLanguages")]
    [ProducesResponseType(typeof(List<InterpreterLanguagesResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAvailableInterpreterLanguages()
    {
        var query = new GetInterpreterLanguages();
        var languages = await queryHandler.Handle<GetInterpreterLanguages, List<InterpreterLanguage>>(query);
        var response = languages.Select(InterpreterLanguageToResponseMapper.MapInterpreterLanguageToResponse).ToList();

        return Ok(response);
    }

    [HttpPut]
    [OpenApiOperation("UpsertInterpreterLanguages")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpsertInterpreterLanguages([FromBody] List<InterpreterLanguagesRequest> request)
    {
        var validator = new BulkUpdateInterpreterLanguagesRequestValidation();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            ModelState.AddFluentValidationErrors(validationResult.Errors);
            return ValidationProblem(ModelState);
        }

        var command = new UpsertInterpreterLanguagesCommand(request.Select(l =>
            new UpsertLanguageDto(l.Code, l.Value, l.WelshValue, (InterpreterType)l.Type, l.Live)).ToList());
        await commandHandler.Handle(command);

        return NoContent();
    }
}