using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("staffmember")]
    [ApiVersion("1.0")]
    [ApiController]
    public class StaffMemberController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public StaffMemberController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Find staff member with contact email matching a search term.
        /// </summary>
        /// <param name="term">Partial string to match contact email with, case-insensitive.</param>
        /// <returns>Person list</returns>
        [HttpGet]
        [OpenApiOperation("GetStaffMemberBySearchTerm")]
        [ProducesResponseType(typeof(IList<PersonResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetStaffMemberBySearchTerm(string term)
        {
            if(term.Length < 3)
            {
                ModelState.AddModelError(nameof(term), "Search term must be at least 3 characters.");
                return ValidationProblem(ModelState);
            }

            var query = new GetStaffMemberBySearchTermQuery(term);
            var staffMemberList = await _queryHandler.Handle<GetStaffMemberBySearchTermQuery, List<Person>>(query);
            if(staffMemberList.Count == 0)
            {
                return NotFound();
            }
            var mapper = new PersonToResponseMapper();
            var response = staffMemberList.Select(x => mapper.MapPersonToResponse(x)).OrderBy(o => o.ContactEmail).ToList();
            return Ok(response);
        }
    }
}