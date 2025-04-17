using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Services;
using BookingsApi.Mappings.V1;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class WorkAllocationsController(
        IHearingAllocationService hearingAllocationService,
        IQueryHandler queryHandler,
        IEventPublisher eventPublisher)
        : ControllerBase
    {
        /// <summary>
        /// Automatically allocates a user to a hearing
        /// </summary>
        /// <param name="hearingId">Id of the hearing to allocate to</param>
        /// <returns>Details of the allocated user</returns>
        [HttpPost("{hearingId}/allocations/automatic")]
        [OpenApiOperation("AllocateHearingAutomatically")]
        [ProducesResponseType(typeof(JusticeUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(SerializableError), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AllocateHearingAutomatically(Guid hearingId)
        {
            var justiceUser = await hearingAllocationService.AllocateAutomatically(hearingId);
            
            if (justiceUser == null)
                return NotFound();

            var justiceUserResponse = JusticeUserToResponseMapper.Map(justiceUser);

            return Ok(justiceUserResponse);
        }
        
        /// <summary>
        /// Get the allocated cso for the hearings of today by venue
        /// </summary>
        /// <param name="hearingVenueNames">Hearing Venue Name array</param>
        /// <returns>list of hearing Ids with the allocated cso</returns>
        [HttpPost("get-allocation/venues")]
        [OpenApiOperation("GetAllocationsForHearingsByVenue")]
        [ProducesResponseType(typeof(IList<AllocatedCsoResponse>), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAllocationsForHearingsByVenue([FromBody]string[] hearingVenueNames)
        {
            var query = new GetHearingsForTodayQuery(hearingVenueNames);
            var hearings = await queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(query);
            return Ok(hearings.Select(e => new AllocatedCsoResponse
            {
                HearingId = e.Id,
                Cso = e.AllocatedTo != null ? JusticeUserToResponseMapper.Map(e.AllocatedTo) : null,
                SupportsWorkAllocation = e.HearingVenue.IsWorkAllocationEnabled
            }));
        }

        /// <summary>
        /// Search for hearings to be allocated via search parameters
        /// </summary>
        /// <param name="searchRequest">Search criteria</param>
        /// <returns>list of hearings matching search criteria</returns>
        [HttpGet("allocation/search")]
        [OpenApiOperation("SearchForAllocationHearings")]
        [ProducesResponseType(typeof(List<HearingAllocationsResponse>), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SearchForAllocationHearings([FromQuery] SearchForAllocationHearingsRequest searchRequest)
        {
            var query = new GetAllocationHearingsBySearchQuery(
                searchRequest.CaseNumber,
                searchRequest.CaseType,
                searchRequest.FromDate,
                searchRequest.ToDate,
                searchRequest.Cso,
                searchRequest.IsUnallocated,
                includeWorkHours: true);
            
            var hearings = await queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);
            
            if (hearings == null || hearings.Count == 0)
                return Ok(new List<HearingAllocationsResponse>());

            var dtos = hearingAllocationService.CheckForAllocationClashes(hearings);
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
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AllocateHearingManually([FromBody] UpdateHearingAllocationToCsoRequest postRequest)
        {
            var allocatedHearings = await hearingAllocationService.AllocateHearingsToCso(postRequest.Hearings, postRequest.CsoId);
            
            var hearingAllocationClashResultDtos = hearingAllocationService.CheckForAllocationClashes(allocatedHearings);
            
            // need to broadcast acknowledgment message for the allocation
            await PublishAllocationsToServiceBus(allocatedHearings, allocatedHearings[0].AllocatedTo);
            
            return Ok(hearingAllocationClashResultDtos.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList());
        }
        
        private async Task PublishAllocationsToServiceBus(List<VideoHearing> hearings, JusticeUser justiceUser)
        {
            var todaysHearing = hearings.Where(x => x.ScheduledDateTime.Date == DateTime.UtcNow.Date).ToList();
            if(todaysHearing.Count != 0)
            {
                await eventPublisher.PublishAsync(new HearingsAllocatedIntegrationEvent(todaysHearing, justiceUser));
            }
        }
    }
}