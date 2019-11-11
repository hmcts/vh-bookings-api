using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("persons")]
    [ApiController]
    public class PersonsController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public PersonsController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
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
        public async Task<IActionResult> PostPersonBySearchTerm([FromBody] string term)
        {
            var query = new GetPersonBySearchTermQuery(term);
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

        private static PersonSuitabilityAnswerResponse BuildResponse(Hearing hearing, string username)
        {
            var participant = hearing.Participants.FirstOrDefault(p => p.Person.Username.ToLower() == username.Trim().ToLower());
            var answers = participant.Questionnaire != null ? participant.Questionnaire.SuitabilityAnswers : new List<SuitabilityAnswer>();
            var suitabilityAnswerToResponseMapper = new SuitabilityAnswerToResponseMapper();
            var personSuitabilityAnswer = new PersonSuitabilityAnswerResponse
            {
                HearingId = hearing.Id,
                ParticipantId = participant.Id,
                UpdatedAt = participant.Questionnaire?.UpdatedDate ?? DateTime.MinValue,
                ScheduledAt = hearing.ScheduledDateTime,
                Answers = suitabilityAnswerToResponseMapper.MapToResponses(answers),
                QuestionnaireNotRequired = hearing.QuestionnaireNotRequired
            };

            return personSuitabilityAnswer;
        }
    }
}