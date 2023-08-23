using System.Diagnostics.CodeAnalysis;
using BookingsApi.Contract.V1.Responses;
namespace BookingsApi.Controllers.V1
{
    
    [ExcludeFromCodeCoverage]
    [Produces("application/json")]
    [Route("hearingRoles")]
    [ApiVersion("1.0")]
    [ApiController]
    public class HearingRolesController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        public HearingRolesController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }
        /// <summary>
        /// Get hearing roles
        /// </summary>
        /// <returns>Returns case independent hearing roles</returns>
        [HttpGet]
        [OpenApiOperation("GetHearingRoles")]
        [ProducesResponseType(typeof(List<HearingRoleResponse>), (int) HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingRoles()
        {            
            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            
            var response = hearingRoles
                .Where(hr => hr.Live)
                .Select(x => new HearingRoleResponse { Name = x.Name, UserRole = x.UserRole?.Name.ToString() })
                .ToList();

            return Ok(response);
        }
    }
}