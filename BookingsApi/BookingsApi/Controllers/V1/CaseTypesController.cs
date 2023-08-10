using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("casetypes")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CaseTypesController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        public CaseTypesController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        ///     Get available case types
        /// </summary>
        /// <returns>A list of available case types</returns>
        [HttpGet]
        [OpenApiOperation("GetCaseTypes")]
        [ProducesResponseType(typeof(List<CaseTypeResponse>), (int) HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCaseTypes()
        {
            var query = new GetAllCaseTypesQuery(hideExpired:true);
            var caseTypes = await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query);
            var response = caseTypes.Select(caseType => new CaseTypeResponse
                {
                    Id = caseType.Id,
                    Name = caseType.Name,
                    ServiceId = caseType.ServiceId, //ServiceId Property behind refData toggle
                    HearingTypes = caseType.HearingTypes.Where(ht => ht.Live).Select(hearingType => new HearingTypeResponse
                    {
                        Id = hearingType.Id,
                        Name = hearingType.Name,
                        Code = hearingType.Code
                    }).ToList()
                }
            );
            return Ok(response);
        } 
        
        /// <summary>
        /// Get case roles for a case type
        /// </summary>
        /// <param name="caseTypeParam"></param>
        /// <returns>Available case roles for given case type</returns>
        [HttpGet("{caseTypeParam}/caseroles")]
        [OpenApiOperation("GetCaseRolesForCaseType")]
        [ProducesResponseType(typeof(List<CaseRoleResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCaseRolesForCaseType(string caseTypeParam)
        {
            var query = new GetCaseRolesForCaseTypeQuery(caseTypeParam);
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(query);

            if (caseType == null)
            {
                return NotFound();
            }

            var response = caseType.CaseRoles.Select(x => new CaseRoleResponse
            {
                Name = x.Name
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Get hearing roles for a case role of a case type
        /// </summary>
        /// <param name="caseTypeName">Hearing case type</param>
        /// <param name="caseRoleName">Hearing case role</param>
        /// <returns>Available hearing roles for given case role</returns>
        [HttpGet("{caseTypeName}/caseroles/{caseRoleName}/hearingroles")]
        [OpenApiOperation("GetHearingRolesForCaseRole")]
        [ProducesResponseType(typeof(List<HearingRoleResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingRolesForCaseRole(string caseTypeName, string caseRoleName)
        {            
            var query = new GetCaseRolesForCaseTypeQuery(caseTypeName);
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(query);

            if (caseType == null)
            {
                return NotFound();
            }

            var caseRole = caseType.CaseRoles.SingleOrDefault(x => x.Name == caseRoleName);
            if (caseRole == null)
            {
                return NotFound();
            }

            var response = caseRole.HearingRoles.Where(hr => hr.Live).Select(x => new HearingRoleResponse
            {
                Name = x.Name, UserRole = x.UserRole?.Name.ToString()
            }).ToList();

            return Ok(response);
        }

    }
}