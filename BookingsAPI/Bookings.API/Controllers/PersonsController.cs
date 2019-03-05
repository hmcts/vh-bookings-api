using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [ProducesResponseType(typeof(PersonResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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
        [ProducesResponseType(typeof(PersonResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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
    }
}