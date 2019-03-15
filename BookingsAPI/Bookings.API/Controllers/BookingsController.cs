using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.API.Utilities;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class BookingsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private const string HearingsListsEndpointBaseUrl = "hearings/";

        public BookingsController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        ///     Gets list of upcoming bookings hearing for a given case types
        /// </summary>
        /// <param name="types">The hearing case types.</param>
        /// <param name="cursor">Cursor specifying from which entries to read next page, is defaulted if not specified</param>
        /// <param name="limit">The max number hearings records to return.</param>
        /// <returns>The list of bookings video hearing</returns>
        [HttpGet("types", Name = "GetHearingsByTypes")]
        [SwaggerOperation(OperationId = "GetHearingsByTypes")]
        [ProducesResponseType(typeof(BookingsHearingResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BookingsHearingResponse>> GetHearingsByTypes([FromQuery(Name = "types")]List<int> types, [FromQuery]string cursor = "0", [FromQuery]int limit = 100)
        {
            const string BookingsEndpointUrl = "types";
            types = types ?? new List<int>();
            if (await ValidateCaseTypes(types))
            {
                ModelState.AddModelError("Hearing types", "Invalid value for hearing types");
                return BadRequest(ModelState);
            }

            var query = new GetBookingsByCaseTypesQuery(types, cursor, limit);
            var videoHearings = await _queryHandler.Handle<GetBookingsByCaseTypesQuery, IList<VideoHearing>>(query);
            var mapper = new VideoHearingsToBookingsResponseMapper();
            var response = new PaginationCursorBasedBuilder<BookingsResponse, VideoHearing>(mapper.MapHearingResponses)
               .WithSourceItems(videoHearings.AsQueryable())
               .Limit(limit)
               .CaseTypes(types)
               .Cursor(cursor)
               .ResourceUrl(HearingsListsEndpointBaseUrl + BookingsEndpointUrl)
               .Build();

            return Ok(response);
        }

        private async Task<bool> ValidateCaseTypes(List<int> caseTypes)
        {
            var query = new GetAllCaseTypesQuery();
            var allCasesTypes = await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query);
            return caseTypes.Count == 0 || caseTypes.All(s => allCasesTypes.Any(x => x.Id == s));
        }
    }
}
