using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("Participants")]
    [ApiVersion("1.0")]
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
        [HttpGet("username/{username}")]
        [OpenApiOperation("GetParticipantsByUsername")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
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