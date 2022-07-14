using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain.RefData;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("casetypes")]
    [ApiController]
    public class CaseTypesController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IFeatureToggles _featureFlag;

        public CaseTypesController(IQueryHandler queryHandler, IFeatureToggles featureToggles)
        {
            _queryHandler = queryHandler;
            _featureFlag = featureToggles;
        }

        /// <summary>
        ///     Get available case types
        /// </summary>
        /// <returns>A list of available case types</returns>
        [HttpGet]
        [OpenApiOperation("GetCaseTypes")]
        [ProducesResponseType(typeof(List<CaseTypeResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetCaseTypes()
        {
            var query = new GetAllCaseTypesQuery();
            var caseTypes = await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query);
            IEnumerable<CaseTypeResponse> response;
            //ServiceId Property behind refData toggle
            if (_featureFlag.ReferenceDataToggle())
                response = caseTypes.Select(caseType => new CaseTypeResponse
                    {
                        Id = caseType.Id,
                        Name = caseType.Name,
                        ServiceId = caseType.ServiceId,
                        HearingTypes = caseType.HearingTypes.Where(ht => ht.Live).Select(hearingType => new HearingTypeResponse
                        {
                            Id = hearingType.Id,
                            Name = hearingType.Name
                            
                        }).ToList()
                    }
                );
            else
                response = caseTypes.Select(caseType => new CaseTypeResponse
                    {
                        Id = caseType.Id,
                        Name = caseType.Name,
                        HearingTypes = caseType.HearingTypes.Where(ht => ht.Live).Select(hearingType => new HearingTypeResponse
                        {
                            Id = hearingType.Id,
                            Name = hearingType.Name
                            
                        }).ToList()
                    }
                );

            return Ok(response);
        } 
        
        /// <summary>
        /// Get case roles for a case type
        /// </summary>
        /// <param name="caseTypeName"></param>
        /// <returns>Available case roles for given case type</returns>
        [HttpGet("{caseTypeParam}/caseroles")]
        [OpenApiOperation("GetCaseRolesForCaseType")]
        [ProducesResponseType(typeof(List<CaseRoleResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCaseRolesForCaseType(string caseTypeParam)
        {
            var query = new GetCaseTypeQuery(caseTypeParam);
            var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);

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
        public async Task<IActionResult> GetHearingRolesForCaseRole(string caseTypeName, string caseRoleName)
        {            
            var query = new GetCaseTypeQuery(caseTypeName);
            var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);

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