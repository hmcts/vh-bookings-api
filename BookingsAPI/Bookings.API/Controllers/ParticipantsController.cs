using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Commands;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class ParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public ParticipantsController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Get a participants in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <returns>List of participants</returns>
        [HttpGet("{hearingId}/participants", Name = "GetAllParticipantsInHearing")]
        [SwaggerOperation(OperationId = "GetAllParticipantsInHearing")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllParticipantsInHearing(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);

                var mapper = new ParticipantToResponseMapper();
                var response = participants.Select(x => mapper.MapParticipantToResponse(x)).ToList();

                return Ok(response);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Get a single participant in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <param name="participantId">The Id of the participant</param>
        /// <returns>The participant</returns>
        [HttpGet("{hearingId}/participants/{participantId}", Name = "GetParticipantInHearing")]
        [SwaggerOperation(OperationId = "GetParticipantInHearing")]
        [ProducesResponseType(typeof(ParticipantResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetParticipantInHearing(Guid hearingId, Guid participantId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }
            
            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }
            
            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
                var participant = participants.FirstOrDefault(x => x.Id == participantId);

                if (participant == null)
                {
                    return NotFound();
                }

                var mapper = new ParticipantToResponseMapper();
                var response = mapper.MapParticipantToResponse(participant);

                return Ok(response);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add participant(s) to a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param> 
        /// <param name="request">The participant information to add</param>
        /// <returns>The participant</returns>
        [HttpPut("{hearingId}/participants", Name = "AddParticipantsToHearing")]
        [SwaggerOperation(OperationId = "AddParticipantsToHearing")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddParticipantsToHearing(Guid hearingId,
            [FromBody] AddParticipantsToHearingRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new AddParticipantsToHearingRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var mapper = new ParticipantRequestToNewParticipantMapper();
            var participants = request.Participants
                .Select(x => mapper.MapRequestToParticipant(x, videoHearing.CaseType)).ToList();
            var command = new AddParticipantsToVideoHearingCommand(hearingId, participants);
            await _commandHandler.Handle(command);

            return NoContent();
        }

        /// <summary>
        /// Remove a participant from a hearing
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}/participants/{participantId}", Name = "RemoveParticipantFromHearing")]
        [SwaggerOperation(OperationId = "RemoveParticipantFromHearing")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveParticipantFromHearing(Guid hearingId, Guid participantId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }
            
            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }
            
            var query = new GetParticipantsInHearingQuery(hearingId);
            var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
            var participant = participants.FirstOrDefault(x => x.Id == participantId);

            if (participant == null)
            {
                return NotFound();
            }
            
            var command = new RemoveParticipantFromHearingCommand(hearingId, participant);
            await _commandHandler.Handle(command);

            return NoContent();
        }
    }
}