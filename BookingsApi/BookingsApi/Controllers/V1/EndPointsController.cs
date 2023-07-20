using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Extensions;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Mappings;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
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
        [OpenApiOperation("AddEndPointToHearing")]
        [ProducesResponseType(typeof(EndpointResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  AddEndPointToHearingAsync(Guid hearingId, AddEndpointRequest addEndpointRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = await new AddEndpointRequestValidation().ValidateAsync(addEndpointRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            try
            {
                var newEp = EndpointToResponseMapper.MapRequestToNewEndpointDto(addEndpointRequest, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem);

                var command = new AddEndPointToHearingCommand(hearingId, newEp);
                await _commandHandler.Handle(command);

                var hearing =
                    await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                var endpoint = hearing.GetEndpoints().FirstOrDefault(x => x.DisplayName.Equals(addEndpointRequest.DisplayName));
                if (endpoint != null)
                {
                    var endpointResponse = EndpointToResponseMapper.MapEndpointToResponse(endpoint); 
                    
                    if (hearing.Status == BookingStatus.Created)
                    {
                        await _eventPublisher.PublishAsync(new EndpointAddedIntegrationEvent(hearingId, endpoint));    
                    }

                    return Ok(endpointResponse);
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
        [OpenApiOperation("RemoveEndPointFromHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  RemoveEndPointFromHearingAsync(Guid hearingId, Guid endpointId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            try
            {   
                var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
                if(hearing == null) throw new HearingNotFoundException(hearingId);
                var command = new RemoveEndPointFromHearingCommand(hearingId, endpointId);
                await _commandHandler.Handle(command);
                var ep = hearing.GetEndpoints().FirstOrDefault(x => x.Id == endpointId);
                if (ep != null && hearing.Status == BookingStatus.Created)
                {
                    await _eventPublisher.PublishAsync(new EndpointRemovedIntegrationEvent(hearingId, ep.Sip));
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
        [OpenApiOperation("UpdateDisplayNameForEndpoint")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  UpdateEndpointAsync(Guid hearingId, Guid endpointId, UpdateEndpointRequest updateEndpointRequest)
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
                if(hearing == null) throw new HearingNotFoundException(hearingId);
                var defenceAdvocate =
                    DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(updateEndpointRequest.DefenceAdvocateContactEmail,
                        hearing.GetParticipants());
                var command = new UpdateEndPointOfHearingCommand(hearingId, endpointId, updateEndpointRequest.DisplayName, defenceAdvocate);
                await _commandHandler.Handle(command);

                var endpoint = hearing.GetEndpoints().SingleOrDefault(x => x.Id == endpointId);

                if (endpoint != null && hearing.Status == BookingStatus.Created)
                {

                    await _eventPublisher.PublishAsync(new EndpointUpdatedIntegrationEvent(hearingId, endpoint.Sip,
                        updateEndpointRequest.DisplayName, defenceAdvocate?.Person.ContactEmail));
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
