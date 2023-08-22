using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("hearingvenues")]
    [ApiVersion("1.0")]
    [ApiController]
    public class HearingVenuesController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public HearingVenuesController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Get all hearing venues available for booking
        /// </summary>
        /// <returns>List of hearing venues</returns>
        [HttpGet]
        [OpenApiOperation("GetHearingVenues")]
        [ProducesResponseType(typeof(List<HearingVenueResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingVenues([FromQuery] bool excludeExpiredVenue = false)
        {
            var query = new GetHearingVenuesQuery(excludeExpiredVenue);
            var hearingVenues = await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query);

            var response = hearingVenues.Select(x => new HearingVenueResponse
            {
                Id = x.Id, Name = x.Name, Code = x.VenueCode
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Get all hearing venues, include expired venues for hearings taking place today
        /// </summary>
        /// <returns>List of hearing venues</returns>
        [HttpGet("today")]
        [OpenApiOperation("GetHearingVenuesForHearingsToday")]  
        [ProducesResponseType(typeof(List<HearingVenueResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingVenuesIncludingExpiredVenuesForHearingsToday()
        {
            var hearingsToday = await _queryHandler.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(new GetHearingsForTodayQuery());
            var hearingVenues = await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(new GetHearingVenuesQuery());

            var response = hearingVenues.Where(e =>
                e.ExpirationDate == null || e.ExpirationDate > DateTime.Today || hearingsToday.Exists(h => h.HearingVenueName == e.Name));

            return Ok(response.Select(x => new HearingVenueResponse { Id = x.Id, Name = x.Name }).ToList());
        }

        /// <summary>
        /// Get today's hearing venues by their allocated csos
        /// </summary>
        /// <returns>List of hearing venues</returns>
        [HttpGet("Allocated")]
        [OpenApiOperation("GetHearingVenuesByAllocatedCso")]
        [ProducesResponseType(typeof(IList<string>), (int) HttpStatusCode.OK)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetHearingVenueNamesByAllocatedCso([FromQuery] IEnumerable<Guid> csoIds)
        {
            var query = new GetAllocationHearingsBySearchQuery(cso: csoIds, fromDate: DateTime.Today);
            var hearings = await _queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);
            if (hearings == null || !hearings.Any())
                return Ok(new List<string>());
            return Ok(hearings.Select(vh => vh.HearingVenueName));
        }
    }
}