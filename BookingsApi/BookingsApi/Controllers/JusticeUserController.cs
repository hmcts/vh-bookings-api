using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("justiceuser")]
    [ApiController]
    public class JusticeUserController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public JusticeUserController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Find justice user with matching username.
        /// </summary>
        /// <param name="username">String to match the username with.</param>
        /// <returns>Person list</returns>
        [HttpGet("GetJusticeUserByUsername")]
        [OpenApiOperation("GetJusticeUserByUsername")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetJusticeUserByUsername(string username)
        {
            var query = new GetJusticeUserByUsernameQuery(username);
            var justiceUser =
                await _queryHandler.Handle<GetJusticeUserByUsernameQuery, JusticeUser>(query);

            if (justiceUser == null) 
                return NotFound();

            var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);

            return Ok(justiceUserResponse);
        }
    }
}