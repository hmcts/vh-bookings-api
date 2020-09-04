using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("persons")]
    [ApiController]
    public class PersonsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public PersonsController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Get a person by username
        /// </summary>
        /// <param name="username">The username of the person</param>
        /// <returns>Person</returns>
        [HttpGet("username/{username}", Name = "GetPersonByUsername")]
        [SwaggerOperation(OperationId = "GetPersonByUsername")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        [HttpGet("username/hearings", Name = "GetHearingsByUsernameForDeletion")]
        [SwaggerOperation(OperationId = "GetHearingsByUsernameForDeletion")]
        [ProducesResponseType(typeof(List<HearingsByUsernameForDeletionResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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
        }

        /// <summary>
        /// Get a person by contact email
        /// </summary>
        /// <param name="contactEmail">The contact email of the person</param>
        /// <returns>Person</returns>
        [HttpGet("contactEmail/{contactEmail}", Name = "GetPersonByContactEmail")]
        [SwaggerOperation(OperationId = "GetPersonByContactEmail")]
        [ProducesResponseType(typeof(PersonResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        [SwaggerOperation(OperationId = "PostPersonBySearchTerm")]
        [ProducesResponseType(typeof(IList<PersonResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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
        [HttpGet("username/{username}/suitability-answers", Name = "GetPersonSuitabilityAnswers")]
        [SwaggerOperation(OperationId = "GetPersonSuitabilityAnswers")]
        [ProducesResponseType(typeof(List<PersonSuitabilityAnswerResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
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
        [HttpGet("userswithclosedhearings", Name = "GetPersonByClosedHearings")]
        [SwaggerOperation(OperationId = "GetPersonByClosedHearings")]
        [ProducesResponseType(typeof(UserWithClosedConferencesResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPersonByClosedHearings()
        {
            var query = new GetPersonsByClosedHearingsQuery();
            var person = await _queryHandler.Handle<GetPersonsByClosedHearingsQuery, List<string>>(query);
            return Ok(new UserWithClosedConferencesResponse { Usernames = person });
        }

        /// <summary>
        /// Anonymise a person
        /// </summary>
        /// <param name="username">username of person</param>
        /// <returns></returns>
        [HttpPatch("username/{username}/anonymise", Name = "AnonymisePersonWithUsername")]
        [SwaggerOperation(OperationId = "AnonymisePersonWithUsername")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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
    }
}