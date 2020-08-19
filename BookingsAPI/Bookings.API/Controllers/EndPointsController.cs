using Bookings.Api.Contract.Requests;
using Bookings.API.Extensions;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.Common.Configuration;
using Bookings.Common.Services;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Bookings.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class EndPointsController : Controller
    {
        private readonly ICommandHandler _commandHandler;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;

        public EndPointsController(ICommandHandler commandHandler, IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration)
        {
            _commandHandler = commandHandler;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfiguration.Value;
        }

        /// <summary>
        ///  Add an endpoint to a given hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <param name="addEndpointRequest">Details of the endpoint to be added to a hearing</param>
        /// <returns></returns>
        [HttpPost("{hearingId}/endpoints/")]
        [SwaggerOperation(OperationId = "AddEndPointToHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddEndPointToHearing(Guid hearingId, AddEndpointRequest addEndpointRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new AddEndpointRequestValidation().Validate(addEndpointRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var sip = _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
            var pin = _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
            var endPoint = EndpointMapper.MapRequestToEndpoint(addEndpointRequest, $"{sip}{_kinlyConfiguration.SipAddressStem}", pin);

            try
            {
                var command = new AddEndPointFromHearingCommand(hearingId, endPoint);
                await _commandHandler.Handle(command);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }

            return NoContent();
        }

        /// <summary>
        ///  Removes an endpoint from a given hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <param name="endpointId">The endpoint id</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}/endpoints/{endpointId}")]
        [SwaggerOperation(OperationId = "RemoveEndPointFromHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RemoveEndPointFromHearing(Guid hearingId, Guid endpointId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            try
            {
                var command = new RemoveEndPointFromHearingCommand(hearingId, endpointId);
                await _commandHandler.Handle(command);
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
