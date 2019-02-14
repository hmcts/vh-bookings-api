using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("casetypes")]
    [ApiController]
    public class CaseTypesController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public CaseTypesController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Get case roles for a case type
        /// </summary>
        /// <param name="caseTypeName"></param>
        /// <returns>Available case roles for given case type</returns>
        [HttpGet("{caseTypeName}/caseroles")]
        [SwaggerOperation(OperationId = "GetCaseRolesForCaseType")]
        [ProducesResponseType(typeof(List<CaseRoleResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public IActionResult GetCaseRolesForCaseType(string caseTypeName)
        {
            if (string.IsNullOrWhiteSpace(caseTypeName))
            {
                ModelState.AddModelError(nameof(caseTypeName), $"Please provide a valid {nameof(caseTypeName)}");
                return BadRequest(ModelState);
            }
            
            var query = new GetCaseTypeQuery(caseTypeName);
            var caseType = _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);

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
        [SwaggerOperation(OperationId = "GetHearingRolesForCaseRole")]
        [ProducesResponseType(typeof(List<HearingRoleResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public IActionResult GetHearingRolesForCaseRole(string caseTypeName, string caseRoleName)
        {
            if (string.IsNullOrWhiteSpace(caseTypeName))
            {
                ModelState.AddModelError(nameof(caseTypeName), $"Please provide a valid {nameof(caseTypeName)}");
                return BadRequest(ModelState);
            }
            
            if (string.IsNullOrWhiteSpace(caseRoleName))
            {
                ModelState.AddModelError(nameof(caseRoleName), $"Please provide a valid {nameof(caseRoleName)}");
                return BadRequest(ModelState);
            }
            
            var query = new GetCaseTypeQuery(caseTypeName);
            var caseType = _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);

            if (caseType == null)
            {
                return NotFound();
            }

            var caseRole = caseType.CaseRoles.SingleOrDefault(x => x.Name == caseRoleName);
            if (caseRole == null)
            {
                return NotFound();
            }

            var response = caseRole.HearingRoles.Select(x => new HearingRoleResponse()
            {
                Name = x.Name
            }).ToList();

            return Ok(response);
        }

    }
}