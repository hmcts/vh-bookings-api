﻿using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Contract.V2.Enums;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    /// <summary>
    /// A suite of operations to manage JVS endpoints in a booking
    /// </summary>
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EndPointsController : ControllerBase
    {
        private readonly IRandomGenerator _randomGenerator;
        private readonly IQueryHandler _queryHandler;
        private readonly IEndpointService _endpointService;

        public EndPointsController(IRandomGenerator randomGenerator, IQueryHandler queryHandler,
            IEndpointService endpointService)
        {
            _randomGenerator = randomGenerator;
            _queryHandler = queryHandler;
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
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        [Obsolete("This method is deprecated, please use the AddEndPointToHearingV2")]
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
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            var sipAddressStem = _endpointService.GetSipAddressStem((BookingSupplier?)hearing?.ConferenceSupplier);
            var newEp = EndpointToResponseMapper.MapRequestToNewEndpointDto(addEndpointRequest, _randomGenerator, sipAddressStem);
            var endpoint = await _endpointService.AddEndpoint(hearingId, newEp);
            var endpointResponse = EndpointToResponseMapper.MapEndpointToResponse(endpoint);

            return Ok(endpointResponse);
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


            var hearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            if (hearing == null) throw new HearingNotFoundException(hearingId);
            await _endpointService.RemoveEndpoint(hearing, endpointId);


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
        [Obsolete("This method is deprecated, please use the UpdateEndpointV2")]
        public async Task<IActionResult> UpdateEndpointAsync(Guid hearingId, Guid endpointId,
            UpdateEndpointRequest updateEndpointRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return ValidationProblem(ModelState);
            }

            var result = await new UpdateEndpointRequestValidation().ValidateAsync(updateEndpointRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            var hearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            if (hearing == null) throw new HearingNotFoundException(hearingId);
            await _endpointService.UpdateEndpoint(hearing, endpointId,
                updateEndpointRequest.DefenceAdvocateContactEmail, updateEndpointRequest.DisplayName, 
                updateEndpointRequest.InterpreterLanguageCode, updateEndpointRequest.OtherLanguage);


            return NoContent();
        }
    }
}
