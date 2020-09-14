using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
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
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Common.Configuration;
using Bookings.Common.Services;
using Microsoft.Extensions.Options;

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
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;

        public HearingsController(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IEventPublisher eventPublisher, IRandomGenerator randomGenerator, IOptions<KinlyConfiguration> kinlyConfiguration)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfiguration.Value;
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

            var endpoints = new List<Endpoint>();
            if (request.Endpoints != null && request.Endpoints.Count > 0)
            {
                endpoints = request.Endpoints.Select(x =>
                {
                    var sip = _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                    var pin = _randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                    return EndpointToResponseMapper.MapRequestToEndpoint(x, $"{sip}{_kinlyConfiguration.SipAddressStem}", pin);
                }).ToList();
            }

            var createVideoHearingCommand = new CreateVideoHearingCommand(caseType, hearingType,
                request.ScheduledDateTime, request.ScheduledDuration, venue, newParticipants, cases,
                request.QuestionnaireNotRequired, request.AudioRecordingRequired, endpoints)
            {
                HearingRoomName = request.HearingRoomName,
                OtherInformation = request.OtherInformation,
                CreatedBy = request.CreatedBy
            };

            await _commandHandler.Handle(createVideoHearingCommand);

            var videoHearingId = createVideoHearingCommand.NewHearingId;

            var getHearingByIdQuery = new GetHearingByIdQuery(videoHearingId);

            var queriedVideoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

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

            var command = new UpdateHearingCommand(hearingId, request.ScheduledDateTime,
                request.ScheduledDuration, venue, request.HearingRoomName, request.OtherInformation,
                request.UpdatedBy, cases, request.QuestionnaireNotRequired, request.AudioRecordingRequired);

            await _commandHandler.Handle(command);

            var hearingMapper = new HearingToDetailResponseMapper();
            var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            var response = hearingMapper.MapHearingToDetailedResponse(updatedHearing);

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

            var command = new RemoveHearingCommand(hearingId);

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
        /// <param name="request">Status of the hearing to change to</param>
        /// <returns>Success status</returns>
        [HttpPatch("{hearingId}")]
        [SwaggerOperation(OperationId = "UpdateBookingStatus")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UpdateBookingStatus(Guid hearingId, UpdateBookingStatusRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateBookingStatusRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            try
            {
                var bookingStatus = Enum.Parse<BookingStatus>(request.Status.ToString(), true);
                await UpdateHearingStatusAsync(hearingId, bookingStatus, request.UpdatedBy, request.CancelReason);
                
                switch (bookingStatus)
                {
                    case BookingStatus.Created:
                        var queriedVideoHearing = await GetHearingToPublishAsync(hearingId);
                        await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(queriedVideoHearing));
                        break;
                    case BookingStatus.Cancelled:
                        await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(hearingId));
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

        private async Task UpdateHearingStatusAsync(Guid hearingId, BookingStatus bookingStatus, string updatedBy, string cancelReason)
        {
            var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updatedBy, cancelReason);

            await _commandHandler.Handle(command);
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
        /// Anonymises the Hearings, Case, Person and Participant data.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("anonymisehearings")]
        [SwaggerOperation(OperationId = "AnonymiseHearings")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AnonymiseHearingsAsync()
        {
            var anonymiseHearingsCommand = new AnonymiseHearingsCommand();
            await _commandHandler.Handle(anonymiseHearingsCommand);
            return NoContent();
        }

        private async Task<Hearing> GetHearingToPublishAsync(Guid hearingId)
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

        private string BuildCursorPageUrl(string cursor, int limit, List<int> caseTypes)
        {
            const string hearingsListsEndpointBaseUrl = "hearings/";
            const string bookingsEndpointUrl = "types";
            const string resourceUrl = hearingsListsEndpointBaseUrl + bookingsEndpointUrl;

            var types = string.Empty;
            if (caseTypes.Any())
            {
                types = string.Join("&types=", caseTypes);
            }

            return $"{resourceUrl}?types={types}&cursor={cursor}&limit={limit}";
        }

        private async Task<bool> ValidateCaseTypes(List<int> filterCaseTypes)
        {
            if (!filterCaseTypes.Any())
            {
                return true;
            }

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


        /// <summary>
        /// Gets a list of hearing by case number
        /// </summary>
        /// <param name="caseNumber">case number to search by</param>
        /// <returns>list of hearing by case number</returns>
        [HttpGet("audiorecording/casenumber", Name = "GetHearingsByCaseNumber")]
        [SwaggerOperation(OperationId = "GetHearingsByCaseNumber")]
        [ProducesResponseType(typeof(List<HearingsByCaseNumberResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHearingsByCaseNumber([FromQuery]string caseNumber)
        {
            if (caseNumber.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(caseNumber), $"Please provide a valid {nameof(caseNumber)}");
                return BadRequest(ModelState);
            }

            caseNumber = WebUtility.UrlDecode(caseNumber);
            
            var query = new GetHearingsByCaseNumberQuery(caseNumber);
            var hearings = await _queryHandler.Handle<GetHearingsByCaseNumberQuery, List<VideoHearing>>(query);

            var hearingMapper = new HearingByCaseNumberResponseMapper();
            var response = hearingMapper.MapHearingToDetailedResponse(hearings, caseNumber);
            return Ok(response);
        }
    }
}