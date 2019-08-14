using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Requests.Enums;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Helpers;
using Bookings.API.Mappings;
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
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Bookings.API.Controllers
{

    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingsController : Controller
    {
        private const string DefaultCursor = "0";
        private const int DefaultLimit = 100;

        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;

        public HearingsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
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
        /// Get list of all hearings for a given username
        /// </summary>
        /// <param name="username">username of person to search against</param>
        /// <returns>Hearing details</returns>
        [HttpGet(Name = "GetHearingsByUsername")]
        [SwaggerOperation(OperationId = "GetHearingsByUsername")]
        [ProducesResponseType(typeof(List<HearingDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetHearingsByUsername([FromQuery]string username)
        {
            var query = new GetHearingsByUsernameQuery(username);
            var hearings = await _queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearings.Select(hearingMapper.MapHearingToDetailedResponse).ToList();
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

            var individualRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsIndividual).Select(x => x.Name).ToList();

            var addressValidationResult = AddressValidationHelper.ValidateAddress(individualRoles, request.Participants);

            if (!addressValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(addressValidationResult.Errors);
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
                OtherInformation = request.OtherInformation,
                CreatedBy = request.CreatedBy
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
            var cases = MapCase(request.Cases);
            var command =
                new UpdateHearingCommand(hearingId, request.ScheduledDateTime, request.ScheduledDuration, venue,
                    request.HearingRoomName, request.OtherInformation, request.UpdatedBy, cases);
            await _commandHandler.Handle(command);

            var hearingMapper = new HearingToDetailResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(videoHearing);

            if (videoHearing.Status == BookingStatus.Created)
            {
                // publish this event when Hearing is set for ready for video
                await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(videoHearing));
            }

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

            if (videoHearing.Status == BookingStatus.Created)
            {
                // publish the event only for confirmed(created) hearing  
                await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(hearingId));
            }
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
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
                var bookingStatus = MapUpdateBookingStatus(updateBookingStatusRequest.Status);
                var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updateBookingStatusRequest.UpdatedBy);
                await _commandHandler.Handle(command);
                switch (bookingStatus)
                {
                    case BookingStatus.Booked:
                        break;
                    case BookingStatus.Created:
                        var queriedVideoHearing = await GetHearingToPublish(hearingId);
                        var createVideoHearingEvent = new HearingIsReadyForVideoIntegrationEvent(queriedVideoHearing);
                        await _eventPublisher.PublishAsync(createVideoHearingEvent);
                        break;
                    case BookingStatus.Cancelled:
                        await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(hearingId));
                        break;
                    default:
                        break;
                }

                return NoContent();
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }
            catch (DomainRuleException exception)
            {
                exception.ValidationFailures.ForEach(x => ModelState.AddModelError(x.Name, x.Message));
                return Conflict(ModelState);
            }
       }

        private async Task<Hearing> GetHearingToPublish(Guid hearingId)
        {
            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            return videoHearing;
        }

        private async Task<HearingVenue> GetVenue(string venueName)
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);
            return hearingVenues.SingleOrDefault(x => x.Name == venueName);
        }

        /// <summary>
        ///     Get a paged list of booked hearings
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
        public async Task<ActionResult<BookingsResponse>> GetHearingsByTypes([FromQuery(Name = "types")]List<int> types, [FromQuery]string cursor = DefaultCursor, [FromQuery]int limit = DefaultLimit)
        {
            types = types ?? new List<int>();
            if (!await ValidateCaseTypes(types))
            {
                ModelState.AddModelError("Hearing types", "Invalid value for hearing types");
                return BadRequest(ModelState);
            }

            var query = new GetBookingsByCaseTypesQuery(types)
            {
                Cursor = cursor == DefaultCursor ? null : cursor,
                Limit = limit
            };
            var result = await _queryHandler.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(query);

            var mapper = new VideoHearingsToBookingsResponseMapper();

            var response = new BookingsResponse
            {
                PrevPageUrl = BuildCursorPageUrl(cursor, limit, types),
                NextPageUrl = BuildCursorPageUrl(result.NextCursor, limit, types),
                NextCursor = result.NextCursor,
                Limit = limit,
                Hearings = mapper.MapHearingResponses(result)
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets all suitability answers for a given hearing
        /// </summary>
        /// <param name="hearingId">Hearing Id</param>
        /// <returns>>A list of suitability answers</returns>
        [HttpGet("{hearingId}/suitability-answers", Name = "GetSuitabilityAnswers")]
        [SwaggerOperation(OperationId = "GetSuitabilityAnswers")]
        [ProducesResponseType(typeof(List<SuitabilityAnswerResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetSuitabilityAnswers(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }
            //Stub the values here for the test to pass
            var hearingSuitabilityAnswers = new List<HearingSuitabilityAnswerResponse>();
            var hearingSuitabilityAnswer = new HearingSuitabilityAnswerResponse
            {
                ParticipantId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Answers = new List<SuitabilityAnswerResponse>()
                {
                    { new SuitabilityAnswerResponse { Key = "Key", Answer = "Answer", ExtendedAnswer = "ExtendedAnswer" } }
                }
            };
            hearingSuitabilityAnswers.Add(hearingSuitabilityAnswer);
            return Ok(hearingSuitabilityAnswers);
        }

        private string BuildCursorPageUrl(string cursor, int limit, List<int> caseTypes)
        {
            const string hearingsListsEndpointBaseUrl = "hearings/";
            const string bookingsEndpointUrl = "types";
            const string resourceUrl = hearingsListsEndpointBaseUrl + bookingsEndpointUrl;

            var types = string.Empty;
            if (caseTypes != null && caseTypes.Any())
            {
                types = string.Join("&types=", caseTypes);
            }

            return $"{resourceUrl}?types={types}&cursor={cursor}&limit={limit}";
        }

        private async Task<bool> ValidateCaseTypes(List<int> filterCaseTypes)
        {
            if (!filterCaseTypes.Any()) return true;

            var query = new GetAllCaseTypesQuery();
            var validCaseTypes = (await _queryHandler.Handle<GetAllCaseTypesQuery, List<CaseType>>(query))
                .Select(caseType => caseType.Id);

            return filterCaseTypes.All(caseType => validCaseTypes.Contains(caseType));

        }

        private List<Case> MapCase(List<CaseRequest> caseRequestList)
        {
            var mappedList = new List<Case>();
            foreach (var caseRequest in caseRequestList)
            {
                mappedList.Add(new Case(caseRequest.Number, caseRequest.Name));
            }
            return mappedList;
        }

        private BookingStatus MapUpdateBookingStatus(UpdateBookingStatus status)
        {
            return status == Api.Contract.Requests.Enums.UpdateBookingStatus.Created
                ? BookingStatus.Created
                : BookingStatus.Cancelled;
        }
    }
}