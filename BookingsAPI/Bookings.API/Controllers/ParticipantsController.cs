using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
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
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using Bookings.Domain.Validations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bookings.API.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class ParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;

        public ParticipantsController(IQueryHandler queryHandler, ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Get participants in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <returns>List of participants</returns>
        [HttpGet("{hearingId}/participants", Name = "GetAllParticipantsInHearing")]
        [SwaggerOperation(OperationId = "GetAllParticipantsInHearing")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllParticipantsInHearing(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);

                var mapper = new ParticipantToResponseMapper();
                var response = participants.Select(x => mapper.MapParticipantToResponse(x)).ToList();

                return Ok(response);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Get a single participant in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <param name="participantId">The Id of the participant</param>
        /// <returns>The participant</returns>
        [HttpGet("{hearingId}/participants/{participantId}", Name = "GetParticipantInHearing")]
        [SwaggerOperation(OperationId = "GetParticipantInHearing")]
        [ProducesResponseType(typeof(ParticipantResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetParticipantInHearing(Guid hearingId, Guid participantId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                var participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
                var participant = participants.FirstOrDefault(x => x.Id == participantId);

                if (participant == null)
                {
                    return NotFound();
                }

                var mapper = new ParticipantToResponseMapper();
                var response = mapper.MapParticipantToResponse(participant);

                return Ok(response);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Add participant(s) to a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param> 
        /// <param name="request">The participant information to add</param>
        /// <returns>The participant</returns>
        [HttpPost("{hearingId}/participants", Name = "AddParticipantsToHearing")]
        [SwaggerOperation(OperationId = "AddParticipantsToHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AddParticipantsToHearing(Guid hearingId,
            [FromBody] AddParticipantsToHearingRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new AddParticipantsToHearingRequestValidation().Validate(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return BadRequest(ModelState);
            }

            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var caseTypequery = new GetCaseTypeQuery(videoHearing.CaseType.Name);
            var caseType = await _queryHandler.Handle<GetCaseTypeQuery, CaseType>(caseTypequery);

            var individualRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsIndividual).Select(x => x.Name).ToList();

            var addressValidationResult = AddressValidationHelper.ValidateAddress(individualRoles, request.Participants);

            if (!addressValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(addressValidationResult.Errors);
                return BadRequest(ModelState);
            }

            var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            var reprensentatives = request.Participants.Where(x => representativeRoles.Contains(x.HearingRoleName)).ToList();
                
            var representativeValidationResult = RepresentativeValidationHelper.ValidateRepresentativeInfo(reprensentatives);

            if (!representativeValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(representativeValidationResult.Errors);
                return BadRequest(ModelState);
            }


            var mapper = new ParticipantRequestToNewParticipantMapper();
            var participants = request.Participants
                .Select(x => mapper.MapRequestToNewParticipant(x, videoHearing.CaseType)).ToList();
            var command = new AddParticipantsToVideoHearingCommand(hearingId, participants);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Remove a participant from a hearing
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}/participants/{participantId}", Name = "RemoveParticipantFromHearing")]
        [SwaggerOperation(OperationId = "RemoveParticipantFromHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RemoveParticipantFromHearing(Guid hearingId, Guid participantId)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            List<Participant> participants;
            var query = new GetParticipantsInHearingQuery(hearingId);
            try
            {
                participants = await _queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
            }
            catch (HearingNotFoundException)
            {
                return NotFound();
            }

            var participant = participants.FirstOrDefault(x => x.Id == participantId);
            if (participant == null)
            {
                return NotFound();
            }

            var command = new RemoveParticipantFromHearingCommand(hearingId, participant);
            await _commandHandler.Handle(command);

            return NoContent();
        }

        /// <summary>
        /// Update participant details
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <param name="request">The participant information to add</param>
        /// <returns></returns>
        [HttpPut("{hearingId}/participants/{participantId}", Name = "UpdateParticipantDetails")]
        [SwaggerOperation(OperationId = "UpdateParticipantDetails")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateParticipantDetails(Guid hearingId, Guid participantId, [FromBody]UpdateParticipantRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateParticipantRequestValidation().Validate(request);
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

            var participant = videoHearing.GetParticipants().Single(x => x.Id.Equals(participantId));
            if (participant == null)
            {
                return NotFound();
            }

            if (participant.HearingRole.UserRole.IsIndividual)
            {
                var addressValidationResult = new AddressValidation().Validate(request);
                if (!addressValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(result.Errors);
                    return BadRequest(ModelState);
                }
            }

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                var repValidationResult = new RepresentativeValidation().Validate(request);
                if (!repValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(result.Errors);
                    return BadRequest(ModelState);
                }
            }

            var mapper = new UpdateParticipantRequestToNewAddressMapper();
            var address = mapper.MapRequestToNewAddress(request);

            var representativeMapper = new UpdateParticipantRequestToNewRepresentativeMapper();
            var representative = representativeMapper.MapRequestToNewRepresentativeInfo(request);

            var updateParticipantCommand = new UpdateParticipantCommand(participantId, request.Title, 
                request.DisplayName, request.TelephoneNumber, address, 
                request.OrganisationName, videoHearing, representative);

            await _commandHandler.Handle(updateParticipantCommand);

            var updatedParticipant = updateParticipantCommand.UpdatedParticipant;

            var participantMapper = new ParticipantToResponseMapper();
            var response = participantMapper.MapParticipantToResponse(updatedParticipant);

            return Ok(response);
        }

        /// <summary>
        /// Updates suitability answers for the participant
        /// </summary>
        /// <param name="hearingId">Id of hearing</param>
        /// <param name="participantId">Id of participant</param>
        /// <param name="answers">A list of suitability answers to update</param>
        /// <returns>Http status</returns>
        [HttpPut("{hearingId}/participants/{participantId}/suitability-answers", Name = "UpdateSuitabilityAnswers")]
        [SwaggerOperation(OperationId = "UpdateSuitabilityAnswers")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateSuitabilityAnswers(Guid hearingId, Guid participantId, [FromBody]List<SuitabilityAnswersRequest> answers)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            var suitabilityAnswers = answers.Select(answer => new SuitabilityAnswer(answer.Key, answer.Answer, answer.ExtendedAnswer))
                                .ToList();

            var command = new UpdateSuitabilityAnswersCommand(hearingId, participantId, suitabilityAnswers);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }
            catch (HearingNotFoundException)
            {
                return NotFound("Hearing not found");
            }
            catch (ParticipantNotFoundException)
            {
                return NotFound("Participant not found");
            }

            return NoContent();
        }
    }
}
