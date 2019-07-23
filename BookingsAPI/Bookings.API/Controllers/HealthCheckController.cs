using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("HealthCheck")]
    [AllowAnonymous]
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
        [ProducesResponseType(typeof(BookingsApiHealthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BookingsApiHealthResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CheckServiceHealth()
        {
            var response = new BookingsApiHealthResponse();
            try
            {
                var query = new GetHearingVenuesQuery();
                var hearingVenues = await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(query);

                if (!hearingVenues.Any())
                {
                    throw new DataException("Could not retrieve ref data during service health check");
                }

                response.DatabaseHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.DatabaseHealth.Successful = false;
                response.DatabaseHealth.ErrorMessage = ex.Message;
                response.DatabaseHealth.Data = ex.Data;
            }

            return !response.DatabaseHealth.Successful
                ? StatusCode((int) HttpStatusCode.InternalServerError, response)
                : Ok(response);
        }
    }
}