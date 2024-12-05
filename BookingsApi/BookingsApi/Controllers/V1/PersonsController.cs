using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("persons")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PersonsController(
        IQueryHandler queryHandler,
        ICommandHandler commandHandler)
        : ControllerBase
    {
        /// <summary>
        /// Get all hearings for a person by username. Used to list all hearings a person must be removed from before deletion.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("username/hearings")]
        [OpenApiOperation("GetHearingsByUsernameForDeletion")]
        [ProducesResponseType(typeof(List<HearingsByUsernameForDeletionResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingsByUsernameForDeletion([FromQuery] string username)
        {
            var query = new GetHearingsByUsernameForDeletionQuery(username);
            try
            {
                var hearings =
                    await queryHandler.Handle<GetHearingsByUsernameForDeletionQuery, List<VideoHearing>>(query);

                var response = hearings.Select(HearingToUsernameForDeletionResponseMapper.MapToDeletionResponse)
                    .ToList();
                return Ok(response);
            }
            catch (PersonIsAJudgeException)
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get list of person from the old hearings
        /// </summary>
        /// <returns>list of usernames</returns>
        [HttpGet("userswithclosedhearings")]
        [OpenApiOperation("GetPersonByClosedHearings")]
        [ProducesResponseType(typeof(UserWithClosedConferencesResponse), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        [Obsolete("Usage unclear, to be removed in future")]
        public async Task<IActionResult> GetPersonByClosedHearings()
        {
            var query = new GetPersonsByClosedHearingsQuery();
            var person = await queryHandler.Handle<GetPersonsByClosedHearingsQuery, List<string>>(query);
            return Ok(new UserWithClosedConferencesResponse { Usernames = person });
        }

        [HttpGet("getanonymisationdata")]
        [OpenApiOperation("GetAnonymisationData")]
        [ProducesResponseType(typeof(AnonymisationDataResponse), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAnonymisationData()
        {
            var anonymisationData =
                await queryHandler.Handle<GetAnonymisationDataQuery, AnonymisationDataDto>(
                    new GetAnonymisationDataQuery());
            var response = AnonymisationDataResponseMapper.Map(anonymisationData);
            return Ok(response);
        }

        /// <summary>
        /// Anonymise a person
        /// </summary>
        /// <param name="username">username of person</param>
        /// <returns></returns>
        [HttpPatch("username/{username}/anonymise")]
        [OpenApiOperation("AnonymisePersonWithUsername")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AnonymisePerson(string username)
        {
            var command = new AnonymisePersonCommand(username);

            await commandHandler.Handle(command);
            return Ok();
        }

        /// <summary>
        /// Anonymise a person from expired hearing
        /// </summary>
        /// <param name="username">username of person</param>
        /// <returns></returns>
        [HttpPatch("username/{username}/anonymise-for-expired-hearings")]
        [OpenApiOperation("AnonymisePersonWithUsernameForExpiredHearings")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AnonymisePersonWithUsernameForExpiredHearings(string username)
        {
            await commandHandler.Handle(new AnonymisePersonWithUsernameCommand { Username = username });
            return Ok();
        }

        /// <summary>
        /// Updates the person's user name
        /// </summary>
        /// <param name="contactEmail">The contact email of the person</param>
        /// <param name="username">username of the person</param>
        /// <returns>No content</returns>
        [HttpPut("user/{**contactEmail}")]
        [OpenApiOperation("UpdatePersonUsername")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdatePersonUsername(string contactEmail, string username)
        {
            if (!contactEmail.IsValidEmail())
            {
                ModelState.AddModelError(nameof(contactEmail), $"Please provide a valid {nameof(contactEmail)}");
                return ValidationProblem(ModelState);
            }

            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return ValidationProblem(ModelState);
            }

            var query = new GetPersonByContactEmailQuery(contactEmail);
            var person = await queryHandler.Handle<GetPersonByContactEmailQuery, Person>(query);

            if (person == null)
            {
                return NotFound();
            }
            
            var command = new UpdatePersonUsernameCommand(person.Id, username);
            await commandHandler.Handle(command);

            return NoContent();
        }
    }
}