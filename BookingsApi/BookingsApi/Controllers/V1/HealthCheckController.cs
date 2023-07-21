using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
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
        [HttpGet("HealthCheck/health")]
        [HttpGet("health/liveness")]
        [OpenApiOperation("CheckServiceHealth")]
        [ProducesResponseType(typeof(BookingsApiHealthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BookingsApiHealthResponse), (int)HttpStatusCode.InternalServerError)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult>  CheckServiceHealth()
        {
            var response = new BookingsApiHealthResponse
            {
                AppVersion = GetApplicationVersion()
            };

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

        private ApplicationVersion GetApplicationVersion()
        {
            var applicationVersion = new ApplicationVersion();
            applicationVersion.FileVersion = GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
            applicationVersion.InformationVersion = GetExecutingAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);
            return applicationVersion;
        }

        private string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}