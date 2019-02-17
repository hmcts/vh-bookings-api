using System;
using System.Collections.Generic;
using System.Net;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
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
        public IActionResult GetAllParticipantsInHearing(Guid hearingId)
        {
            throw new NotImplementedException();
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
        public IActionResult GetParticipantInHearing(Guid hearingId, Guid participantId)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Add participant(s) to a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <returns>The participant</returns>
        [HttpPut("{hearingId}", Name = "AddParticipantsToHearing")]
        [SwaggerOperation(OperationId = "AddParticipantsToHearing")]
        [ProducesResponseType(typeof(ParticipantResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public IActionResult AddParticipantsToHearing(Guid hearingId, [FromBody] AddParticipantsToHearingRequest request)
        {
            throw new NotImplementedException();
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
        public IActionResult RemoveParticipantFromHearing(Guid hearingId, Guid participantId)
        {
            throw new NotImplementedException();
        }
    }
}