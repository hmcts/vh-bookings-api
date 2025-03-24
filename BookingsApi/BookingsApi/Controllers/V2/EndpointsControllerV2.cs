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
    [HttpPost("v2/hearings/{hearingId}/endpoints/")]
    [OpenApiOperation("AddEndPointToHearingV2")]
    [ProducesResponseType(typeof(EndpointResponseV2), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> AddEndPointToHearingV2Async(Guid hearingId, EndpointRequestV2 addEndpointRequest)
    {
        return await AddEndpointToHearing(hearingId, addEndpointRequest);
    }
    
    /// <summary>
    /// Add an endpoint to a given hearing, used by S&amp;L until they migrate to v2 route
    /// </summary>
    /// <param name="hearingId"></param>
    /// <param name="addEndpointRequest"></param>
    /// <returns></returns>
    [HttpPost("hearings/{hearingId}/endpoints/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(typeof(EndpointResponseV2), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> AddEndPointToHearingV2ForSAndLAsync(Guid hearingId,
        EndpointRequestV2 addEndpointRequest)
    {
        return await AddEndpointToHearing(hearingId, addEndpointRequest);
    }

    private async Task<IActionResult> AddEndpointToHearing(Guid hearingId, EndpointRequestV2 addEndpointRequest)
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
        var newEp = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(addEndpointRequest, randomGenerator, sipAddressStem);
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
    [HttpPatch("v2/hearings/{hearingId}/endpoints/{endpointId}")]
    [OpenApiOperation("UpdateEndpointV2")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateEndpointV2Async(Guid hearingId, Guid endpointId,
        UpdateEndpointRequestV2 updateEndpointRequest)
    {
        return await UpdateEndpoint(hearingId, endpointId, updateEndpointRequest);
    }
    
    /// <summary>
    /// Update an endpoint of a given hearing, used by S&amp;L until they migrate to v2 route
    /// </summary>
    /// <param name="hearingId"></param>
    /// <param name="endpointId"></param>
    /// <param name="updateEndpointRequest"></param>
    /// <returns></returns>
    [HttpPatch("hearings/{hearingId}/endpoints/{endpointId}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateEndpointV2ForSAndLAsync(Guid hearingId, Guid endpointId,
        UpdateEndpointRequestV2 updateEndpointRequest)
    {
        return await UpdateEndpoint(hearingId, endpointId, updateEndpointRequest);
    }

    private async Task<IActionResult> UpdateEndpoint(Guid hearingId, Guid endpointId, UpdateEndpointRequestV2 updateEndpointRequest)
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

        var participantsLinked = updateEndpointRequest.LinkedParticipantEmails;
        participantsLinked.Add(updateEndpointRequest.DefenceAdvocateContactEmail);
        
        await endpointService.UpdateEndpoint(hearing, endpointId,
            new UpdateEndpointDto(participantsLinked,
                updateEndpointRequest.DisplayName,
                updateEndpointRequest.InterpreterLanguageCode, 
                updateEndpointRequest.OtherLanguage, 
                updateEndpointRequest.ExternalParticipantId, 
                updateEndpointRequest.MeasuresExternalId,
                updateEndpointRequest.Screening?.MapToDalDto()));
        
        return NoContent();
    }
}