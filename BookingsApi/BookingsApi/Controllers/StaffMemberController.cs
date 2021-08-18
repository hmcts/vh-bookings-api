using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("staffmember")]
    [ApiController]
    public class StaffMemberController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public StaffMemberController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Find staffmember with contact email matching a search term.
        /// </summary>
        /// <param name="term">Partial string to match contact email with, case-insensitive.</param>
        /// <returns>Person list</returns>
        [HttpPost]
        [OpenApiOperation("PostStaffMemberBySearchTerm")]
        [ProducesResponseType(typeof(IList<PersonResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostStaffMemberBySearchTerm(SearchTermRequest term)
        {
            var query = new GetStaffMemberBySearchTermQuery(term.Term);
            var staffMemberList = await _queryHandler.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(query);
            var mapper = new PersonToResponseMapper();
            var response = staffMemberList.Select(x => mapper.MapPersonToResponse(x)).OrderBy(o => o.ContactEmail).ToList();
            return Ok(response);
        }
    }
}