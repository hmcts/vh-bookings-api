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
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.API.Helpers;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;

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
        private readonly IEventPublisher _eventPublisher;
        private readonly IQueryHandler _queryHandler;
        private readonly KinlyConfiguration _kinlyConfiguration;

        public EndPointsController(ICommandHandler commandHandler, IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration, IEventPublisher eventPublisher, IQueryHandler queryHandler)
        {
            _commandHandler = commandHandler;
            _randomGenerator = randomGenerator;
            _eventPublisher = eventPublisher;
            _queryHandler = queryHandler;
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
        public async Task<IActionResult> AddEndPointToHearingAsync(Guid hearingId, AddEndpointRequest addEndpointRequest)
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

            try
            {
                var hearing =
                    await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));

                var defenceAdvocate =
                    DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(addEndpointRequest, hearing.GetParticipants());
                var sip = _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                var pin = _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                var endPoint = EndpointToResponseMapper.MapRequestToEndpoint(addEndpointRequest,
                    $"{sip}{_kinlyConfiguration.SipAddressStem}", pin, defenceAdvocate);


                var command = new AddEndPointToHearingCommand(hearingId, endPoint);
                await _commandHandler.Handle(command);

                if (hearing.Status == Domain.Enumerations.BookingStatus.Created)
                {
                    await _eventPublisher.PublishAsync(new EndpointAddedIntegrationEvent(hearingId, endPoint));
                }
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
        public async Task<IActionResult> RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId)
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
                var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                if (hearing.Status == Domain.Enumerations.BookingStatus.Created)
                {
                    await _eventPublisher.PublishAsync(new EndpointRemovedIntegrationEvent(hearingId, command.RemoveSip));
                }
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
        [SwaggerOperation(OperationId = "UpdateDisplayNameForEndpoint")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateDisplayNameForEndpointAsync(Guid hearingId, Guid endpointId, UpdateEndpointRequest updateEndpointRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateEndpointRequestValidation().Validate(updateEndpointRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            try
            {
                var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                var defenceAdvocate =
                    DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(updateEndpointRequest, hearing.GetParticipants());
                var command = new UpdateEndPointOfHearingCommand(hearingId, endpointId, updateEndpointRequest.DisplayName, defenceAdvocate);
                await _commandHandler.Handle(command);

                if (hearing.Status == Domain.Enumerations.BookingStatus.Created)
                {
                    var endpoint = hearing.GetEndpoints().Single(x => x.Id == endpointId);

                    await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearingId, endpoint.Sip,
                        updateEndpointRequest.DisplayName, defenceAdvocate?.Person.Username));
                }
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
