using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;
using BookingsApi.Mappings.V2.Extensions;
using BookingsApi.Validations.V2;

namespace BookingsApi.Controllers.V2;
/// <summary>
/// A suite of operations to manage JVS endpoints in a booking (V2)
/// </summary>
[Consumes("application/json")]
[Produces("application/json")]
[Route(template:"v{version:apiVersion}/hearings")]
[ApiVersion("2.0")]
[ApiController]
public class EndpointsControllerV2(
    IEndpointService endpointService,
    IQueryHandler queryHandler,
    IRandomGenerator randomGenerator)
    : ControllerBase
{
    /// <summary>
    ///  Add an endpoint to a given hearing
    /// </summary>
    /// <param name="hearingId">The hearing id</param>
    /// <param name="addEndpointRequest">Details of the endpoint to be added to a hearing</param>
    /// <returns></returns>
    [HttpPost("{hearingId}/endpoints/")]
    [OpenApiOperation("AddEndPointToHearingV2")]
    [ProducesResponseType(typeof(EndpointResponseV2), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> AddEndPointToHearingV2Async(Guid hearingId,
        EndpointRequestV2 addEndpointRequest)
    {
        var result = await new EndpointRequestValidationV2().ValidateAsync(addEndpointRequest);
        if (!result.IsValid)
        {
            ModelState.AddFluentValidationErrors(result.Errors);
            return ValidationProblem(ModelState);
        }

        var hearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
        if (hearing == null)
        {
            throw new HearingNotFoundException(hearingId);
        }
        
        var sipAddressStem = endpointService.GetSipAddressStem(hearing.ConferenceSupplier.MapToContractEnum());
        var newEp = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(addEndpointRequest, randomGenerator,
            sipAddressStem);
        var endpoint = await endpointService.AddEndpoint(hearingId, newEp);
        var endpointResponse = EndpointToResponseV2Mapper.MapEndpointToResponse(endpoint);

        return Ok(endpointResponse);
    }
    
    /// <summary>
    ///  Update an endpoint of a given hearing
    /// </summary>
    /// <param name="hearingId">The hearing id</param>
    /// <param name="endpointId">The endpoint id</param>
    /// <param name="updateEndpointRequest">Details of the endpoint to be updated</param>
    /// <returns></returns>
    [HttpPatch("{hearingId}/endpoints/{endpointId}")]
    [OpenApiOperation("UpdateEndpointV2")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> UpdateEndpointV2Async(Guid hearingId, Guid endpointId,
        UpdateEndpointRequestV2 updateEndpointRequest)
    {
        var result = await new EndpointRequestValidationV2().ValidateAsync(updateEndpointRequest);
        if (!result.IsValid)
        {
            ModelState.AddFluentValidationErrors(result.Errors);
            return ValidationProblem(ModelState);
        }

        var hearing =
            await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
        if (hearing == null) throw new HearingNotFoundException(hearingId);
        await endpointService.UpdateEndpoint(hearing, endpointId,
            updateEndpointRequest.DefenceAdvocateContactEmail, updateEndpointRequest.DisplayName,
            updateEndpointRequest.InterpreterLanguageCode, updateEndpointRequest.OtherLanguage,
            updateEndpointRequest.Screening?.MapToDalDto());
        
        return NoContent();
    }
}