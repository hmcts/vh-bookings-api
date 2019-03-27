using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Mappings;
using Bookings.API.Utilities;
using Bookings.API.Validations;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{

    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public HearingsController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Get details for a given hearing
        /// </summary>
        /// <param name="hearingId">Id for a hearing</param>
        /// <returns>Hearing details</returns>
        [HttpGet("{hearingId}", Name = "GetHearingDetailsById")]
        [SwaggerOperation(OperationId = "GetHearingDetailsById")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetHearingDetailsById(Guid hearingId)
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

            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(videoHearing);
            return Ok(response);
        }

        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "BookNewHearing")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BookNewHearing(BookNewHearingRequest request)
        {
            var result = new BookNewHearingRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var query = new GetCaseTypeQuery(request.CaseTypeName);
            var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(query);

            if (caseType == null)
            {
                ModelState.AddModelError(nameof(request.CaseTypeName), "Case type does not exist");
                return BadRequest(ModelState);
            }

            var hearingType = caseType.HearingTypes.SingleOrDefault(x => x.Name == request.HearingTypeName);
            if (hearingType == null)
            {
                ModelState.AddModelError(nameof(request.HearingTypeName), "Hearing type does not exist");
                return BadRequest(ModelState);
            }

            var venue = await GetVenue(request.HearingVenueName);
            if (venue == null)
            {
                ModelState.AddModelError(nameof(request.HearingVenueName), "Hearing venue does not exist");
                return BadRequest(ModelState);
            }

            var mapper = new ParticipantRequestToNewParticipantMapper();
            var newParticipants = request.Participants.Select(x => mapper.MapRequestToNewParticipant(x, caseType))
                .ToList();
            var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
            var createVideoHearingCommand = new CreateVideoHearingCommand(caseType, hearingType,
                request.ScheduledDateTime, request.ScheduledDuration, venue, newParticipants, cases)
            {
                HearingRoomName = request.HearingRoomName,
                OtherInformation = request.OtherInformation
            };
            await _commandHandler.Handle(createVideoHearingCommand);

            var videoHearingId = createVideoHearingCommand.NewHearingId;

            var getHearingByIdQuery = new GetHearingByIdQuery(videoHearingId);
            var queriedVideoHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);


            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(queriedVideoHearing);
            return CreatedAtAction(nameof(GetHearingDetailsById), new { hearingId = response.Id }, response);
        }


        /// <summary>
        /// Update the details of a hearing such as venue, time and duration
        /// </summary>
        /// <param name="hearingId">The id of the hearing to update</param>
        /// <param name="request">Details to update</param>
        /// <returns>Details of updated hearing</returns>
        [HttpPut("{hearingId}")]
        [SwaggerOperation(OperationId = "UpdateHearingDetails")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateHearingDetails(Guid hearingId, [FromBody] UpdateHearingRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateHearingRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var venue = await GetVenue(request.HearingVenueName);
            if (venue == null)
            {
                ModelState.AddModelError(nameof(request.HearingVenueName), "Hearing venue does not exist");
                return BadRequest(ModelState);
            }

            var command =
                new UpdateHearingCommand(hearingId, request.ScheduledDateTime, request.ScheduledDuration, venue,
                    request.HearingRoomName, request.OtherInformation);
            await _commandHandler.Handle(command);

            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(videoHearing);

            return Ok(response);
        }

        /// <summary>
        /// Remove an existing hearing
        /// </summary>
        /// <param name="hearingId">The hearing id</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}")]
        [SwaggerOperation(OperationId = "RemoveHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveHearing(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var command =
                new RemoveHearingCommand(hearingId);
            await _commandHandler.Handle(command);

            return NoContent();
        }

        /// <summary>
        /// Update booking status
        /// </summary>
        /// <param name="hearingId">Id of the hearing to update the status for</param>
        /// <param name="updateBookingStatusRequest">Status of the hearing to change to</param>
        /// <returns>Success status</returns>
        [HttpPatch("{hearingId}")]
        [SwaggerOperation(OperationId = "UpdateBookingStatus")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateBookingStatus(Guid hearingId, UpdateBookingStatusRequest updateBookingStatusRequest)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateBookingStatusRequestValidation().Validate(updateBookingStatusRequest);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            try
            {
                var bookingStatus = Enum.Parse<BookingStatus>(updateBookingStatusRequest.Status);
                var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updateBookingStatusRequest.UpdatedBy);
                await _commandHandler.Handle(command);
                return Ok();
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }
            catch (DomainRuleException exception)
            {
                exception.ValidationFailures.ForEach(x => ModelState.AddModelError(x.Name, x.Message));
                return BadRequest(ModelState);
            }
        }

        private async Task<HearingVenue> GetVenue(string venueName)
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);
            return hearingVenues.SingleOrDefault(x => x.Name == venueName);
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
        [ProducesResponseType(typeof(BookingsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<BookingsResponse>> GetHearingsByTypes([FromQuery(Name = "types")]List<int> types, [FromQuery]string cursor = "0", [FromQuery]int limit = 100)
        {
            const string HearingsListsEndpointBaseUrl = "hearings/";
            const string BookingsEndpointUrl = "types";
            types = types ?? new List<int>();
            if (!await ValidateCaseTypes(types))
            {
                ModelState.AddModelError("Hearing types", "Invalid value for hearing types");
                return BadRequest(ModelState);
            }

            var query = new GetBookingsByCaseTypesQuery(types, cursor, limit);
            var videoHearings = await _queryHandler.Handle<GetBookingsByCaseTypesQuery, IList<VideoHearing>>(query);
            var items = videoHearings ?? new List<VideoHearing>();
            var mapper = new VideoHearingsToBookingsResponseMapper();
            var response = new PaginationCursorBasedBuilder<BookingsResponse, VideoHearing>(mapper.MapHearingResponses)
               .WithSourceItems(items.AsQueryable())
               .Limit(limit)
               .CaseTypes(types)
               .Cursor(cursor)
               .ResourceUrl(HearingsListsEndpointBaseUrl + BookingsEndpointUrl)
               .Build();

            return Ok(response);
        }

        private async Task<bool> ValidateCaseTypes(List<int> caseTypes)
        {
            var result = false;

            if (!caseTypes.Any())
            {
                result = true;
            }
            else
            {
                var query = new GetAllCaseTypesQuery();
                var allCasesTypes = await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query);
                if (allCasesTypes != null)
                {
                    result = caseTypes.Count == 0 || caseTypes.All(s => allCasesTypes.Any(x => x.Id == s));
                }
            }
            return result;
        }
    }
}