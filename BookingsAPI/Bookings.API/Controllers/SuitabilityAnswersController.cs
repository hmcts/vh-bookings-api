using Bookings.Api.Contract.Responses;
using Bookings.API.Mappings;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain.Participants;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("suitability-answers")]
    [ApiController]
    public class SuitabilityAnswersController : Controller
    {
        private const string DefaultCursor = "0";
        private const int DefaultLimit = 100;

        private readonly IQueryHandler _queryHandler;

        public SuitabilityAnswersController(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        /// <summary>
        ///     Get a cursor based list of suitability answers
        /// </summary>
        /// <param name="cursor">Cursor specifying from which entries to read next page, is defaulted if not specified</param>
        /// <param name="limit">The max number hearings records to return.</param>
        /// <returns>The list of latest suitability answers for participants</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "GetSuitabilityAnswers")]
        [ProducesResponseType(typeof(SuitabilityAnswersResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<SuitabilityAnswersResponse>> GetSuitabilityAnswers([FromQuery]string cursor = DefaultCursor, [FromQuery]int limit = DefaultLimit)
        {
           
            var query = new GetParticipantWithSuitabilityAnswersQuery()
            {
                Cursor = cursor == DefaultCursor ? null : cursor,
                Limit = limit
            };
            var result = await _queryHandler.Handle<GetParticipantWithSuitabilityAnswersQuery, CursorPagedResult<Participant, string>>(query);

            var mapper = new SuitabilityAnswersListToResponseMapper();

            var response = new SuitabilityAnswersResponse
            {
                PrevPageUrl = BuildCursorPageUrl(cursor, limit),
                NextPageUrl = BuildCursorPageUrl(result.NextCursor, limit),
                NextCursor = result.NextCursor,
                Limit = limit,
                ParticipantSuitabilityAnswerResponse = mapper.MapParticipantSuitabilityAnswerResponses(result)
            };

            return Ok(response);
        }

        private string BuildCursorPageUrl(string cursor, int limit)
        {
            const string resourceUrl = "suitability_answer/";
            return $"{resourceUrl}?cursor={cursor}&limit={limit}";
        }
    }
}
