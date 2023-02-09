using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("hearingvenues")]
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
        public async Task<IActionResult> GetHearingVenues()
        {
            var query = new GetHearingVenuesQuery();
            var hearingVenues = await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query);

            var response = hearingVenues.Select(x => new HearingVenueResponse()
            {
                Id = x.Id, Name = x.Name
            }).ToList();

            return Ok(response);
        }
        
        /// <summary>
        /// Get today's hearing venues by their allocated csos
        /// </summary>
        /// <returns>List of hearing venues</returns>
        [HttpGet("Allocated")]
        [OpenApiOperation("GetHearingVenuesByAllocatedCso")]
        [ProducesResponseType(typeof(IList<VenueWithAllocatedCsoReponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetHearingVenueNamesByAllocatedCso([FromQuery] IEnumerable<Guid> csoIds)
        {
            var query = new GetAllocationHearingsBySearchQuery(cso: csoIds, fromDate: DateTime.Today);
            var hearings = await _queryHandler.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(query);
            
            if (hearings == null || !hearings.Any())
                return Ok(new List<VenueWithAllocatedCsoReponse>());
            
            return Ok(hearings.Select(vh => new VenueWithAllocatedCsoReponse()
            {
                HearingVenueName = vh.HearingVenueName,
                Cso = JusticeUserToResponseMapper.Map(vh.AllocatedTo)
            } ));
        }
    }
}