using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace BookingsApi.Controllers.V2;

[Produces("application/json")]
[Route(template:"v{version:apiVersion}/hearings")]
[ApiController]
[ApiVersion("2.0")]
public class HearingsController :ControllerBase
{
    private readonly IQueryHandler _queryHandler;

    public HearingsController(IQueryHandler queryHandler)
    {
        _queryHandler = queryHandler;
    }

    /// <summary>
    /// Get details for a given hearing
    /// </summary>
    /// <param name="hearingId">Id for a hearing</param>
    /// <returns>Hearing details</returns>
    [HttpGet("{hearingId}")]
    [OpenApiOperation("GetHearingDetailsById")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult>  GetHearingDetailsById(Guid hearingId)
    {
        if (hearingId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
            return BadRequest(ModelState);
        }

        var query = new GetHearingByIdQuery(hearingId);
        var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

        if (videoHearing == null)
        {
            return NotFound();
        }
        return Ok("v2");
    }
}