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
    public class WorkAllocationsController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IHearingAllocationService _hearingAllocationService;
        private readonly ILogger<WorkAllocationsController> _logger;
        private readonly IEventPublisher _eventPublisher;

        public WorkAllocationsController(IHearingAllocationService hearingAllocationService, 
            IQueryHandler queryHandler, ILogger<WorkAllocationsController> logger,
            IEventPublisher eventPublisher)
        {
            _hearingAllocationService = hearingAllocationService;
            _queryHandler = queryHandler;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }
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
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetUnallocatedHearings()
        {
            var today = DateTime.UtcNow; //provide a range (from today 1 year) for unallocated hearings rather than return all past and present.
            var query = new GetAllocationHearingsBySearchQuery(isUnallocated: true, fromDate: today, toDate: today.AddYears(1));
            var results = await _queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);

            if (results.Count <= 0)
                _logger.LogInformation("[GetUnallocatedHearings] Could not find any unallocated hearings");
            var response = results.Select(HearingToDetailsResponseMapper.Map).ToList();
            return Ok(response);
        }
         
        /// <summary>
        /// Get the allocated cso for the provided hearing Ids
        /// </summary>
        /// <param name="hearingIds">Hearing Reference ID array</param>
        /// <returns>list of hearing Ids with the allocated cso</returns>
        [HttpPost("get-allocation")]
        [OpenApiOperation("GetAllocationsForHearings")]
        [ProducesResponseType(typeof(IList<AllocatedCsoResponse>), (int)HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAllocationsForHearings([FromBody]Guid[] hearingIds)
        {
            var allocatedHearings = await _queryHandler
                    .Handle<GetAllocationHearingsQuery, List<VideoHearing>>(new GetAllocationHearingsQuery(hearingIds));
            
            return Ok(allocatedHearings.Select(e => new AllocatedCsoResponse
            {
                HearingId = e.Id,
                Cso = e.AllocatedTo != null ? JusticeUserToResponseMapper.Map(e.AllocatedTo) : null,
                SupportsWorkAllocation = e.HearingVenue.IsWorkAllocationEnabled
            }));
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
            var hearings = await _queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(query);
            return Ok(hearings.Select(e => new AllocatedCsoResponse
            {
                HearingId = e.Id,
                Cso = e.AllocatedTo != null ? JusticeUserToResponseMapper.Map(e.AllocatedTo) : null
            }));
        }

        /// <summary>
        /// Search for hearings to be allocate via search parameters
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
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AllocateHearingManually([FromBody] UpdateHearingAllocationToCsoRequest postRequest)
        {
            try
            {
                var list = await _hearingAllocationService.AllocateHearingsToCso(postRequest.Hearings, postRequest.CsoId);
                
                var dtos = _hearingAllocationService.CheckForAllocationClashes(list);
                
                // need to broadcast acknowledgment message for the allocation
                await PublishAllocationsToServiceBus(list, list.First().AllocatedTo);
                
                return Ok(dtos.Select(HearingAllocationResultDtoToAllocationResponseMapper.Map).ToList());
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }
        }
        
        private async Task PublishAllocationsToServiceBus(List<VideoHearing> hearings, JusticeUser justiceUser)
        {
            var todaysHearing = hearings.Where(x => x.ScheduledDateTime.Date == DateTime.UtcNow.Date).ToList();
            if(todaysHearing.Any())
            {
                await _eventPublisher.PublishAsync(new AllocationHearingsIntegrationEvent(todaysHearing, justiceUser));
            }
        }
    }
}