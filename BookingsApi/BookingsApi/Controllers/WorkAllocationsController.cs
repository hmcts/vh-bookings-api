using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Validations;
using BookingsApi.Extensions;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class WorkAllocationsController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IHearingAllocationService _hearingAllocationService;
        private readonly ILogger<WorkAllocationsController> _logger;

        public WorkAllocationsController(IHearingAllocationService hearingAllocationService, IQueryHandler queryHandler, ILogger<WorkAllocationsController> logger)
        {
            _hearingAllocationService = hearingAllocationService;
            _queryHandler = queryHandler;
            _logger = logger;
        }
        /// <summary>
        /// Automatically allocates a user to a hearing
        /// </summary>
        /// <param name="hearingId">Id of the hearing to allocate to</param>
        /// <returns>Details of the allocated user</returns>
        [HttpPost("{hearingId}/allocations/automatic")]
        [OpenApiOperation("AllocateHearingAutomatically")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AllocateHearingAutomatically(Guid hearingId)
        {
            try
            {
                var justiceUser = await _hearingAllocationService.AllocateAutomatically(hearingId);
                
                if (justiceUser == null)
                    return NotFound();

                var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);

                return Ok(justiceUserResponse);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }
        }
        
        /// <summary>
        /// Get all the unallocated hearings
        /// </summary>
        /// <returns>unallocated hearings</returns>
        [HttpGet("unallocated")]
        [OpenApiOperation("GetUnallocatedHearings")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUnallocatedHearings()
        {
            var query = new GetAllocationHearingsBySearchQuery(isUnallocated: true);
            var results = await _queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);

            if (results.Count <= 0)
                _logger.LogInformation("[GetUnallocatedHearings] Could not find any unallocated hearings");
            var response = results.Select(HearingToDetailsResponseMapper.Map).ToList();
            return Ok(response);
        }
 
        /// <summary>
        /// Search for hearings to be allocate via search parameters
        /// </summary>
        /// <param name="searchRequest">Search criteria</param>
        /// <returns>list of hearings matching search criteria</returns>
        [HttpGet("allocation/search", Name = "SearchForAllocationHearings")]
        [OpenApiOperation("SearchForAllocationHearings")]
        [ProducesResponseType(typeof(List<HearingAllocationsResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchForAllocationHearings([FromQuery] SearchForAllocationHearingsRequest searchRequest)
        {
            var query = new GetAllocationHearingsBySearchQuery(
                searchRequest.CaseNumber, 
                searchRequest.CaseType, 
                searchRequest.FromDate, 
                searchRequest.ToDate, 
                searchRequest.Cso,
                searchRequest.IsUnallocated);
            
            var hearings = await _queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);
            
            if (hearings == null || !hearings.Any())
                return Ok(new List<HearingAllocationsResponse>());

            var dtos = _hearingAllocationService.CheckForAllocationClashes(hearings);
            return Ok(dtos.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList());
        }

        /// <summary>
        /// Allocate list of hearings to a CSO user 
        /// </summary>
        /// <returns>list of allocated hearings</returns>
        [HttpPatch("allocations")]
        [OpenApiOperation("AllocateHearingsToCso")]
        [ProducesResponseType(typeof(List<HearingAllocationsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateHearingManually([FromBody] UpdateHearingAllocationToCsoRequest postRequest)
        {
            try
            {
                var list = await _hearingAllocationService.AllocateHearingsToCso(postRequest.Hearings, postRequest.CsoId);
                
                var dtos = _hearingAllocationService.CheckForAllocationClashes(list);
                return Ok(dtos.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList());
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }
        }

    }
}