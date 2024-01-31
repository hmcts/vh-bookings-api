using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EndPointsController : Controller
    {
        private readonly IRandomGenerator _randomGenerator;
        private readonly IQueryHandler _queryHandler;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IHearingEndpointService _hearingEndpointService;

        public EndPointsController(IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration, IQueryHandler queryHandler,
            IHearingEndpointService hearingEndpointService)
        {
            _randomGenerator = randomGenerator;
            _queryHandler = queryHandler;
            _kinlyConfiguration = kinlyConfiguration.Value;
            _hearingEndpointService = hearingEndpointService;
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
        public async Task<IActionResult> AddEndPointToHearingAsync(Guid hearingId, AddEndpointRequest addEndpointRequest)
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
                var endpoint = await _hearingEndpointService.AddEndpointToHearing(hearingId, newEp);
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
                var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                if(hearing == null) throw new HearingNotFoundException(hearingId);
                await _hearingEndpointService.RemoveEndpointFromHearing(hearing, endpointId);
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
        ///  Update an endpoint of a given hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <param name="endpointId">The endpoint id</param>
        /// <param name="updateEndpointRequest">Details of the endpoint to be updated</param>
        /// <returns></returns>
        [HttpPatch("{hearingId}/endpoints/{endpointId}")]
        [OpenApiOperation("UpdateDisplayNameForEndpoint")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateEndpointAsync(Guid hearingId, Guid endpointId, UpdateEndpointRequest updateEndpointRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }

            var result = new UpdateEndpointRequestValidation().Validate(updateEndpointRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            try
            {
                var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                if(hearing == null) throw new HearingNotFoundException(hearingId);

                await _hearingEndpointService.UpdateEndpointOfHearing(hearing, endpointId, 
                    updateEndpointRequest.DisplayName, updateEndpointRequest.DefenceAdvocateContactEmail);
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
    }
}
