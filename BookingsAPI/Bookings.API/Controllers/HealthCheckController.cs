using Bookings.DAL.Queries;
using Bookings.Domain;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.DAL.Queries.Core;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("HealthCheck")]
    [ApiController]
    public class HealthCheckController : Controller
    {
        private readonly IQueryHandler _queryHandler;

        public HealthCheckController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        ///     Run a health check of the service
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("health")]
        [SwaggerOperation(OperationId = "CheckServiceHealth")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Health()
        {
            try
            {
                var query = new GetHearingVenuesQuery();
                var hearingVenues = await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query);

                if (hearingVenues.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                var data = new
                {
                    ex.Message,
                    ex.Data
                };
                return StatusCode((int)HttpStatusCode.InternalServerError, data);
            }

            return Ok();
        }
    }
}