using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.API.Extensions;
using Bookings.API.Mappings;
using Bookings.API.Validations;
using Bookings.DAL.Commands;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.RefData;
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
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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

            var response = MapHearingToDetailResponseModel(videoHearing);
            return Ok(response);
        }

        private HearingDetailsResponse MapHearingToDetailResponseModel(Hearing videoHearing)
        {
            var cases = videoHearing.GetCases()
                .Select(x => new CaseResponse
                {
                    Name = x.Name,
                    Number = x.Number,
                    IsLeadCase = x.IsLeadCase
                })
                .ToList();

            var participants = videoHearing.GetParticipants()
                .Select(x => new ParticipantResponse
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    CaseRoleName = x.CaseRole.Name,
                    HearingRoleName = x.HearingRole.Name,
                    UserRoleName = x.HearingRole.UserRole.Name,
                    Title = x.Person.Title,
                    FirstName = x.Person.FirstName,
                    LastName = x.Person.LastName,
                    MiddleNames = x.Person.MiddleNames,
                    Username = x.Person.Username,
                    ContactEmail = x.Person.ContactEmail,
                    TelephoneNumber = x.Person.TelephoneNumber
                })
                .ToList();


            var response = new HearingDetailsResponse
            {
                Id = videoHearing.Id,
                ScheduledDuration = videoHearing.ScheduledDuration,
                ScheduledDateTime = videoHearing.ScheduledDateTime,
                HearingTypeName = videoHearing.HearingType.Name,
                CaseTypeName = videoHearing.CaseType.Name,
                HearingVenueName = videoHearing.HearingVenueName,
                Cases = cases,
                Participants = participants
            };
            return response;
        }

        /// <summary>
        /// Request to book a new hearing
        /// </summary>
        /// <param name="request">Details of a new hearing to book</param>
        /// <returns>Details of the newly booked hearing</returns>
        [HttpPost]
        [SwaggerOperation(OperationId = "BookNewHearing")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
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

            var videoHearing = new VideoHearing(caseType, hearingType, request.ScheduledDateTime,
                request.ScheduledDuration, venue);

            var cases = request.Cases.Select(x => new Case(x.Number, x.Name)).ToList();
            videoHearing.AddCases(cases);

            var mapper = new ParticipantRequestToDomainMap();
            var participants = request.Participants.Select(x => mapper.MapRequestToDomain(x, caseType)).ToList();
            videoHearing.AddParticipants(participants);

            var command = new SaveVideoHearingCommand(videoHearing);
            await _commandHandler.Handle(command);

            var response = MapHearingToDetailResponseModel(videoHearing);
            return CreatedAtAction(nameof(GetHearingDetailsById), new {hearingId = response.Id}, response);
        }


        /// <summary>
        /// Update the details of a hearing such as venue, time and duration
        /// </summary>
        /// <param name="hearingId">The id of the hearing to update</param>
        /// <param name="request">Details to update</param>
        /// <returns>Details of updated hearing</returns>
        [HttpPut("{hearingId}")]
        [SwaggerOperation(OperationId = "UpdateHearingDetails")]
        [ProducesResponseType(typeof(HearingDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
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
                new UpdateHearingCommand(hearingId, request.ScheduledDateTime, request.ScheduledDuration, venue);
            await _commandHandler.Handle(command);

            var response = MapHearingToDetailResponseModel(videoHearing);

            return Ok(response);
        }

        private async Task<HearingVenue> GetVenue(string venueName)
        {
            var getHearingVenuesQuery = new GetHearingVenuesQuery();
            var hearingVenues =
                await _queryHandler.Handle<GetHearingVenuesQuery, List<HearingVenue>>(getHearingVenuesQuery);
            return hearingVenues.SingleOrDefault(x => x.Name == venueName);
        }
    }
}