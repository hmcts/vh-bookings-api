using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("persons")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PersonsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher, ILogger<PersonsController> logger)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Get a person by username
        /// </summary>
        /// <param name="username">The username of the person</param>
        /// <returns>Person</returns>
        [HttpGet("username/{**username}")]
        [OpenApiOperation("GetPersonByUsername")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPersonByUsername(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var query = new GetPersonByUsernameQuery(username);
            var person = await _queryHandler.Handle<GetPersonByUsernameQuery, Person>(query);
            if (person == null)
            {
                return NotFound();
            }

            var mapper = new PersonToResponseMapper();
            var response = mapper.MapPersonToResponse(person);
            return Ok(response);
        }

        /// <summary>
        /// Get all hearings for a person by username
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
                    await _queryHandler.Handle<GetHearingsByUsernameForDeletionQuery, List<VideoHearing>>(query);

                var response = hearings.Select(HearingToUsernameForDeletionResponseMapper.MapToDeletionResponse)
                    .ToList();
                return Ok(response);
            }
            catch (PersonNotFoundException)
            {
                return NotFound();
            }
            catch (PersonIsAJudgeException)
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Get a person by contact email
        /// </summary>
        /// <param name="contactEmail">The contact email of the person</param>
        /// <returns>Person</returns>
        [HttpGet("contactEmail/{**contactEmail}")]
        [OpenApiOperation("GetPersonByContactEmail")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPersonByContactEmail(string contactEmail)
        {
            if (!contactEmail.IsValidEmail())
            {
                ModelState.AddModelError(nameof(contactEmail), $"Please provide a valid {nameof(contactEmail)}");
                return BadRequest(ModelState);
            }

            var query = new GetPersonByContactEmailQuery(contactEmail);
            var person = await _queryHandler.Handle<GetPersonByContactEmailQuery, Person>(query);
            if (person == null)
            {
                return NotFound();
            }

            var mapper = new PersonToResponseMapper();
            var response = mapper.MapPersonToResponse(person);
            return Ok(response);
        }

        /// <summary>
        /// Find persons with contact email matching a search term.
        /// </summary>
        /// <param name="term">Partial string to match contact email with, case-insensitive.</param>
        /// <returns>Person list</returns>
        [HttpPost]
        [OpenApiOperation("PostPersonBySearchTerm")]
        [ProducesResponseType(typeof(IList<PersonResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> PostPersonBySearchTerm(SearchTermRequest term)
        {
            var query = new GetPersonBySearchTermQuery(term.Term);
            var personList = await _queryHandler.Handle<GetPersonBySearchTermQuery, List<Person>>(query);
            var mapper = new PersonToResponseMapper();
            var response = personList.Select(x => mapper.MapPersonToResponse(x)).OrderBy(o => o.ContactEmail).ToList();
            return Ok(response);
        }

        /// <summary>
        /// Get a list of suitability answers for a given person
        /// </summary>
        /// <param name="username">The username of the person</param>
        /// <returns>A list of suitability answers</returns>
        [HttpGet("username/{username}/suitability-answers")]
        [OpenApiOperation("GetPersonSuitabilityAnswers")]
        [ProducesResponseType(typeof(List<PersonSuitabilityAnswerResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Obsolete("This method is deprecated.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPersonSuitabilityAnswers(string username)
        {
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }

            var query = new GetHearingsByUsernameQuery(username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);

            var personSuitabilityAnswers = hearings.Select(hearing => BuildResponse(hearing, username)).Where(s => s != null).ToList();

            return Ok(personSuitabilityAnswers);
        }

        /// <summary>
        /// Get list of person from the old hearings
        /// </summary>
        /// <returns>list of usernames</returns>
        [HttpGet("userswithclosedhearings")]
        [OpenApiOperation("GetPersonByClosedHearings")]
        [ProducesResponseType(typeof(UserWithClosedConferencesResponse), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPersonByClosedHearings()
        {
            var query = new GetPersonsByClosedHearingsQuery();
            var person = await _queryHandler.Handle<GetPersonsByClosedHearingsQuery, List<string>>(query);
            return Ok(new UserWithClosedConferencesResponse { Usernames = person });
        }

        [HttpGet("getanonymisationdata")]
        [OpenApiOperation("GetAnonymisationData")]
        [ProducesResponseType(typeof(AnonymisationDataResponse), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAnonymisationData()
        {
            var anonymisationData =
                await _queryHandler.Handle<GetAnonymisationDataQuery, AnonymisationDataDto>(
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
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AnonymisePerson(string username)
        {
            var command = new AnonymisePersonCommand(username);
            try
            {
                await _commandHandler.Handle(command);
                return Ok();
            }
            catch (PersonNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Anonymise a person from expired hearing
        /// </summary>
        /// <param name="username">username of person</param>
        /// <returns></returns>
        [HttpPatch("username/{username}/anonymise-for-expired-hearings")]
        [OpenApiOperation("AnonymisePersonWithUsernameForExpiredHearings")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AnonymisePersonWithUsernameForExpiredHearings(string username)
        {
            try
            {
                await _commandHandler.Handle(new AnonymisePersonWithUsernameCommand { Username = username });
                return Ok();
            }
            catch (PersonNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to update a person because the {username} does not exist", username);
                return NotFound();
            }
        }
        
        [HttpGet]
        [OpenApiOperation("SearchForNonJudgePersonsByContactEmail")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string),(int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails),(int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string),(int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<PersonResponse>> SearchForNonJudgePersonsByContactEmail([FromQuery]string contactEmail)
        {
            if (!contactEmail.IsValidEmail())
            {
                ModelState.AddModelError(nameof(contactEmail), $"Please provide a valid {nameof(contactEmail)}");
                return BadRequest(ModelState);
            }

            var personQuery = new GetPersonByContactEmailQuery(contactEmail);
            var person = await _queryHandler.Handle<GetPersonByContactEmailQuery, Person>(personQuery);
            if (person == null)
            {
                return NotFound($"Person with {contactEmail} does not exist");
            }

            var hearingsQuery = new GetHearingsByUsernameQuery(person.Username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(hearingsQuery);

            var judicialHearings = hearings.SelectMany(v => v.Participants.Where(p => p.PersonId == person.Id))
                .Any(x => x.HearingRole.UserRole.IsJudge);

            if (judicialHearings)
            {
                return Unauthorized("Only searches for non Judge persons are allowed");
            }
            
            var mapper = new PersonToResponseMapper();
            var response = mapper.MapPersonToResponse(person);
            return Ok(response);
        }
        
        /// <summary>
        /// Update the personal details
        /// </summary>
        /// <param name="personId">The id of the person to update</param>
        /// <param name="payload">Updated details of the person</param>
        /// <returns></returns>
        [HttpPut("{personId}")]
        [OpenApiOperation("UpdatePersonDetails")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<PersonResponse>>> UpdatePersonDetails([FromRoute] Guid personId, [FromBody] UpdatePersonDetailsRequest payload)
        {
            var validation = await new UpdatePersonDetailsRequestValidation().ValidateAsync(payload);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return BadRequest(ModelState);
            }

            try
            {
                var command = new UpdatePersonCommand(personId, payload.FirstName, payload.LastName, payload.Username);
                await _commandHandler.Handle(command);
            }
            catch (PersonNotFoundException e)
            {
                _logger.LogError(e, "Failed to update a person because the person {Person} does not exist", personId);
                return NotFound($"{personId} does not exist");
            }

            // get all hearings for user
            var query = new GetHearingsByUsernameQuery(payload.Username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);
            
            // raise an update event for each hearing to ensure consistency between video and bookings api
            var anonymisedText = "@hmcts.net";
            var nonAnonymisedParticipants = hearings
                .Where(x => x.Status == BookingStatus.Created &&
                            x.GetCases().Any(c => !c.Name.EndsWith(anonymisedText))).SelectMany(c => c.Participants)
                .Where(p => p.PersonId == personId && !p.DisplayName.EndsWith(anonymisedText)).ToList();
            _logger.LogDebug("Updating {Count} non-anonymised participants", nonAnonymisedParticipants.Count);

            foreach(var participant in nonAnonymisedParticipants)
            {
                // map to updated participant event
                await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(participant.HearingId, participant));
            }
     
            return Accepted();
        }
        
        private static PersonSuitabilityAnswerResponse BuildResponse(Hearing hearing, string username)
        {
            PersonSuitabilityAnswerResponse personSuitabilityAnswer = null;
            if (hearing.Participants != null) {
                var participant = hearing.Participants.FirstOrDefault(p => p.Person.Username.ToLower() == username.Trim().ToLower());
                if (participant != null)
                {
                    var answers = participant.Questionnaire != null ? participant.Questionnaire.SuitabilityAnswers : new List<SuitabilityAnswer>();
                    var suitabilityAnswerToResponseMapper = new SuitabilityAnswerToResponseMapper();
                    personSuitabilityAnswer = new PersonSuitabilityAnswerResponse
                    {
                        HearingId = hearing.Id,
                        ParticipantId = participant.Id,
                        UpdatedAt = participant.Questionnaire?.UpdatedDate ?? DateTime.MinValue,
                        ScheduledAt = hearing.ScheduledDateTime,
                        Answers = suitabilityAnswerToResponseMapper.MapToResponses(answers),
                        QuestionnaireNotRequired = hearing.QuestionnaireNotRequired
                    };
                }
            }

            return personSuitabilityAnswer;
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
                return BadRequest(ModelState);
            }
            if (!username.IsValidEmail())
            {
                ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                return BadRequest(ModelState);
            }
            var query = new GetPersonByContactEmailQuery(contactEmail);
            var person = await _queryHandler.Handle<GetPersonByContactEmailQuery, Person>(query);

            if (person == null)
            {
                return NotFound();
            }

            try
            {
                var command = new UpdatePersonUsernameCommand(person.Id, username);
                await _commandHandler.Handle(command);
            }
            catch (PersonNotFoundException e)
            {
                _logger.LogError(e, "Failed to update a person because the person with {contactEmail} does not exist", contactEmail);
                return NotFound($"{contactEmail} does not exist");
            }

            return NoContent();
        }
    }
}