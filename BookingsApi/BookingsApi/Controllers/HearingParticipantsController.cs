using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Domain.Validations;
using BookingsApi.Extensions;
using BookingsApi.Helpers;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Mappings;
using BookingsApi.Validations;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BookingsApi.Controllers
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiController]
    public class HearingParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;

        public HearingParticipantsController(IQueryHandler queryHandler,
            ICommandHandler commandHandler,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Get participants in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <returns>List of participants</returns>
        [HttpGet("{hearingId}/participants", Name = "GetAllParticipantsInHearing")]
        [OpenApiOperation("GetAllParticipantsInHearing")]
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
        [OpenApiOperation("GetParticipantInHearing")]
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
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
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
        [OpenApiOperation("AddParticipantsToHearing")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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

            var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            var representatives = request.Participants.Where(x => representativeRoles.Contains(x.HearingRoleName)).ToList();

            var representativeValidationResult = RepresentativeValidationHelper.ValidateRepresentativeInfo(representatives);

            if (!representativeValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(representativeValidationResult.Errors);
                return BadRequest(ModelState);
            }

            var participants = request.Participants
                .Select(x => ParticipantRequestToNewParticipantMapper.Map(x, videoHearing.CaseType)).ToList();
            var linkedParticipants =
                LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);
            
            var command = new AddParticipantsToVideoHearingCommand(hearingId, participants, linkedParticipants);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }

            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);
            await PublishEventForNewParticipantsAsync(hearing, participants);

            var addedParticipants = hearing.Participants.Where(x => request.Participants.Select(p => p.ContactEmail).Contains(x.Person.ContactEmail));

            var response = CreateParticipantResponseList(addedParticipants);

            return Ok(response);
        }

        /// <summary>
        /// Updates a hearings participants
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param> 
        /// <param name="request">The participants information</param>
        /// <returns>204 No Content</returns>
        [HttpPost("{hearingId}/updateParticipants", Name = "UpdateHearingParticipants")]
        [OpenApiOperation("UpdateHearingParticipants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateHearingParticipants(Guid hearingId,
            [FromBody] UpdateHearingParticipantsRequest request)
        {
            if (hearingId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(hearingId), $"Please provide a valid {nameof(hearingId)}");
                return BadRequest(ModelState);
            }

            var result = new UpdateHearingParticipantsRequestValidation().Validate(request);
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

            var representativeRoles = caseType.CaseRoles.SelectMany(x => x.HearingRoles).Where(x => x.UserRole.IsRepresentative).Select(x => x.Name).ToList();
            var representatives = request.NewParticipants.Where(x => representativeRoles.Contains(x.HearingRoleName)).ToList();

            var representativeValidationResult = RepresentativeValidationHelper.ValidateRepresentativeInfo(representatives);

            if (!representativeValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(representativeValidationResult.Errors);
                return BadRequest(ModelState);
            }

            var newParticipants = request.NewParticipants
                .Select(x => ParticipantRequestToNewParticipantMapper.Map(x, videoHearing.CaseType)).ToList();

            var existingParticipants = videoHearing.Participants.Where(x => request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id));

            var existingParticipantDetails = new List<ExistingParticipantDetails>();

            foreach (var existingParticipantRequest in request.ExistingParticipants)
            {
                var existingParticipant = existingParticipants.SingleOrDefault(ep => ep.Id == existingParticipantRequest.ParticipantId);

                if (existingParticipant == null)
                {
                    continue;
                }

                var existingParticipantDetail = new ExistingParticipantDetails
                {
                    DisplayName = existingParticipantRequest.DisplayName,
                    OrganisationName = existingParticipantRequest.OrganisationName,
                    ParticipantId = existingParticipantRequest.ParticipantId,
                    Person = existingParticipant.Person,
                    RepresentativeInformation = new RepresentativeInformation { Representee = existingParticipantRequest.Representee },
                    TelephoneNumber = existingParticipantRequest.TelephoneNumber,
                    Title = existingParticipantRequest.Title
                };

                existingParticipantDetails.Add(existingParticipantDetail);
            }
             

            var linkedParticipants =
                LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);

            var command = new UpdateHearingParticipantsCommand(hearingId, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (DomainRuleException e)
            {
                ModelState.AddDomainRuleErrors(e.ValidationFailures);
                return BadRequest(ModelState);
            }

            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);
            await PublishEventForUpdateParticipantsAysnc(hearing, existingParticipantDetails, newParticipants, 
                request.RemovedParticipantIds, linkedParticipants);

            var upsertedParticipants = hearing.Participants.Where(x => request.NewParticipants.Select(p => p.ContactEmail).Contains(x.Person.ContactEmail)
                || request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id));

            var response = CreateParticipantResponseList(upsertedParticipants);

            return Ok(response);
        }

        /// <summary>
        /// Remove a participant from a hearing
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <returns></returns>
        [HttpDelete("{hearingId}/participants/{participantId}", Name = "RemoveParticipantFromHearing")]
        [OpenApiOperation("RemoveParticipantFromHearing")]
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

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound();
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
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

            // ONLY publish this event when Hearing is set for ready for video
            if (videoHearing.Status == BookingStatus.Created)
            {
                await _eventPublisher.PublishAsync(new ParticipantRemovedIntegrationEvent(hearingId, participantId));
            }

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
        [OpenApiOperation("UpdateParticipantDetails")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return BadRequest(ModelState);
            }

            var result = await new UpdateParticipantRequestValidation().ValidateAsync(request);
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
            Participant participant = null;
            var participants = videoHearing.GetParticipants();
            if (participants != null)
            {
                participant = participants.SingleOrDefault(x => x.Id.Equals(participantId));
            }

            if (participant == null)
            {
                return NotFound();
            }

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                var test = new RepresentativeValidation();
                await test.ValidateAsync(request);
                var repValidationResult = await new RepresentativeValidation().ValidateAsync(request);
                if (!repValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(result.Errors);
                    return BadRequest(ModelState);
                }
            }

            var representative =
                UpdateParticipantRequestToNewRepresentativeMapper.MapRequestToNewRepresentativeInfo(request);

            var linkedParticipants =
                LinkedParticipantRequestToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);
            
            var updateParticipantCommand = new UpdateParticipantCommand(hearingId, participantId, request.Title,
                request.DisplayName, request.TelephoneNumber,
                request.OrganisationName, representative, linkedParticipants);

            await _commandHandler.Handle(updateParticipantCommand);

            var updatedParticipant = updateParticipantCommand.UpdatedParticipant;

            var participantMapper = new ParticipantToResponseMapper();

            ParticipantResponse response = null;
            if (updatedParticipant != null)
            {
                response = participantMapper.MapParticipantToResponse(updatedParticipant);
            }

            // ONLY publish this event when Hearing is set for ready for video
            if (videoHearing.Status == BookingStatus.Created)
            { 
                await _eventPublisher.PublishAsync(new ParticipantUpdatedIntegrationEvent(hearingId, updatedParticipant));
            }

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
        [OpenApiOperation("UpdateSuitabilityAnswers")]
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

            // Reject any requests with duplicate keys
            var duplicateKeyFound = answers.GroupBy(x => x.Key).Any(g => g.Count() > 1);
            if (duplicateKeyFound)
            {
                ModelState.AddModelError(nameof(participantId), $"Request '{nameof(answers)}' cannot contain duplicate keys.");
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

        private async Task PublishEventForNewParticipantsAsync(Hearing hearing, IEnumerable<NewParticipant> newParticipants)
        {
            var participants = hearing.GetParticipants()
                        .Where(x => newParticipants.Any(y => y.Person.ContactEmail == x.Person.ContactEmail)).ToList();
            if (participants.Any())
            {
                if(hearing.Status == BookingStatus.Created) 
                {
                    await _eventPublisher.PublishAsync(new ParticipantsAddedIntegrationEvent(hearing, participants));
                }
                else if (participants.Any(x => x.HearingRole.UserRole.Name == "Judge"))
                {
                    await UpdateHearingStatusAsync(hearing.Id, BookingStatus.Created, "System", string.Empty);
                    await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(hearing, participants));
                }
                else
                {
                    await _eventPublisher.PublishAsync(new CreateAndNotifyUserIntegrationEvent(hearing, participants));
                    await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(hearing, participants));
                }
            }
        }
        private async Task UpdateHearingStatusAsync(Guid hearingId, BookingStatus bookingStatus, string updatedBy, string cancelReason)
        {
            var command = new UpdateHearingStatusCommand(hearingId, bookingStatus, updatedBy, cancelReason);
            await _commandHandler.Handle(command);
        }
        private async Task PublishEventForUpdateParticipantsAysnc(Hearing hearing, List<ExistingParticipantDetails> existingParticipants, List<NewParticipant> newParticipants,
            List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants)
        {
            var eventNewParticipants = hearing.GetParticipants()
                        .Where(x => newParticipants.Any(y => y.Person.ContactEmail == x.Person.ContactEmail)).ToList();
            if (eventNewParticipants.Any() || removedParticipantIds.Any())
            {
                if (hearing.Status == BookingStatus.Created)
                {
                    var eventExistingParticipants = hearing.GetParticipants()
                        .Where(x => existingParticipants.Any(y => y.ParticipantId == x.Id)).ToList();
                
                    var eventLinkedParticipants = new List<Infrastructure.Services.Dtos.LinkedParticipantDto>();
                
                    foreach (var linkedParticipant in linkedParticipants)
                    {
                        var primaryLinkedParticipant = hearing.GetParticipants().SingleOrDefault(x => x.Person.ContactEmail == linkedParticipant.ParticipantContactEmail);
                        var secondaryLinkedParticipant = hearing.GetParticipants().SingleOrDefault(x => x.Person.ContactEmail == linkedParticipant.LinkedParticipantContactEmail);
                
                        eventLinkedParticipants.Add(new Infrastructure.Services.Dtos.LinkedParticipantDto
                        {
                            LinkedId = secondaryLinkedParticipant.Id,
                            ParticipantId = primaryLinkedParticipant.Id,
                            Type = linkedParticipant.Type
                        });
                    }
                
                    var hearingParticipantsUpdatedIntegrationEvent = new HearingParticipantsUpdatedIntegrationEvent(hearing, eventExistingParticipants, eventNewParticipants,
                        removedParticipantIds, eventLinkedParticipants);
                    await _eventPublisher.PublishAsync(hearingParticipantsUpdatedIntegrationEvent);
                }
                else if (eventNewParticipants.Any(x => x.HearingRole.UserRole.Name == "Judge"))
                {
                    await UpdateHearingStatusAsync(hearing.Id, BookingStatus.Created, "System", string.Empty);
                    await _eventPublisher.PublishAsync(new HearingIsReadyForVideoIntegrationEvent(hearing, eventNewParticipants));
                }
                else
                {
                    if (eventNewParticipants.Any())
                    {
                        await _eventPublisher.PublishAsync(new CreateAndNotifyUserIntegrationEvent(hearing, eventNewParticipants));
                        await _eventPublisher.PublishAsync(new HearingNotificationIntegrationEvent(hearing, eventNewParticipants));   
                    }
                }
            }
        }

        private List<ParticipantResponse> CreateParticipantResponseList(IEnumerable<Participant> participants)
        {
            if (participants.Any())
            {
                var mapper = new ParticipantToResponseMapper();
                return participants.Select(x => mapper.MapParticipantToResponse(x)).ToList();
            }

            return new List<ParticipantResponse>();
        }
    }
}
