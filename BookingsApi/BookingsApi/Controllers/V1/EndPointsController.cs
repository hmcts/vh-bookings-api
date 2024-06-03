using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EndPointsController : ControllerBase
    {
        private readonly IRandomGenerator _randomGenerator;
        private readonly IQueryHandler _queryHandler;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IEndpointService _endpointService;

        public EndPointsController(IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration, IQueryHandler queryHandler,
            IEndpointService endpointService)
        {
            _randomGenerator = randomGenerator;
            _queryHandler = queryHandler;
            _kinlyConfiguration = kinlyConfiguration.Value;
            _endpointService = endpointService;
        }

        /// <summary>
        ///  Add an endpoint to a given hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <param name="addEndpointRequest">Details of the endpoint to be added to a hearing</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/endpoints/")]
        [OpenApiOperation("AddEndPointToHearing")]
        [ProducesResponseType(typeof(EndpointResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddEndPointToHearingAsync(Guid hearingId,
            AddEndpointRequest addEndpointRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }

            var result = await new AddEndpointRequestValidation().ValidateAsync(addEndpointRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            try
            {
                var newEp = EndpointToResponseMapper.MapRequestToNewEndpointDto(addEndpointRequest, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem);
                var endpoint = await _endpointService.AddEndpoint(hearingId, newEp);
                var endpointResponse = EndpointToResponseMapper.MapEndpointToResponse(endpoint);

                return Ok(endpointResponse);

            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
        }

        /// <summary>
        ///  Removes an endpoint from a given hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <param name="endpointId">The endpoint id</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}/endpoints/{endpointId}")]
        [OpenApiOperation("RemoveEndPointFromHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }

            try
            {
                var hearing =
                    await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                if (hearing == null) throw new HearingNotFoundException(hearingId);
                await _endpointService.RemoveEndpoint(hearing, endpointId);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (EndPointNotFoundException exception)
            {
                return NotFound(exception.Message);
            }

            return NoContent();
        }

        /// <summary>
        ///  Currently returns a V2 response, this will be updated one V1 controllers refactored
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <returns></returns>
        [HttpGet("endpoints/{sipAddress}")]
        [OpenApiOperation("GetEndpoint")]
        [ProducesResponseType(typeof(EndpointResponseV2), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetEndpoint(string sipAddress)
        {
            var endpoint = await _queryHandler.Handle<GetEndpointQuery, Endpoint>(new GetEndpointQuery(sipAddress));
            if (endpoint == null)
                return NotFound("No endpoint found with the provided SipAddress");
            return Ok(EndpointToResponseV2Mapper.MapEndpointToResponse(endpoint));
        }
    }
}
