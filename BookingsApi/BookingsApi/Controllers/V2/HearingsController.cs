using System;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V2
{
    [Produces("application/json")]
    [Route(template:"v{version:apiVersion}/hearings")]
    [ApiController]
    [ApiVersion("2.0")]
    public class HearingsController : ControllerBase
    {
        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [OpenApiOperation("BookNewHearingWithCode")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("2.0")]
        public IActionResult BookNewHearing(BookNewHearingRequest request)
        {
            throw new NotImplementedException("Remove the V1 namespace");
        }
    }
}