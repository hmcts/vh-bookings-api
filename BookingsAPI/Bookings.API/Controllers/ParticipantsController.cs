using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.Participants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("Participants")]
    [ApiController]
    public class ParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public ParticipantsController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Get a participants by username
        /// </summary>
        /// <param name="username">The username of the participant</param>
        /// <returns>Participant</returns>
        [HttpGet("username/{username}", Name = "GetParticipantsByUsername")]
        [SwaggerOperation(OperationId = "GetParticipantsByUsername")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetParticipantsByUsername(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var query = new GetParticipantsByUsernameQuery(username);
            var participants = await _queryHandler.Handle<GetParticipantsByUsernameQuery, List<Participant>>(query);

            if (participants == null || participants.Count == 0)
            {
                return NotFound();
            }

            var mapper = new ParticipantToResponseMapper();

            return Ok(participants.Select(x => mapper.MapParticipantToResponse(x)));
        }
    }
}