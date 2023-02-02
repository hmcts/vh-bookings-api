using System.Collections.Generic;
using System.Linq;
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
        
        /// <summary>
        /// Get list of Justice User filtered by passed term. If term is null then not filter applied.
        /// </summary>
        /// <param name="term">term to filter result</param>
        /// <returns>Justice User list</returns>
        [HttpGet("GetJusticeUserList")]
        [OpenApiOperation("GetJusticeUserList")]
        [ProducesResponseType(typeof(List<JusticeUserResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetJusticeUserList(string term)
        {
            var query = new GetJusticeUserListQuery(term);
            var userList =
                await _queryHandler.Handle<GetJusticeUserListQuery, List<JusticeUser>>(query);
            
            var list = userList.Select(user => JusticeUserToResponseMapper.Map(user));

            return Ok(list.ToList());
        }
    }
}