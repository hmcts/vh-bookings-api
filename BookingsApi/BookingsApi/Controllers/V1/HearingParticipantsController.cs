using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Helpers;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.Common;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class HearingParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IEventPublisher _eventPublisher;
        private readonly IHearingParticipantService _hearingParticipantService;

        public HearingParticipantsController(IQueryHandler queryHandler,
            ICommandHandler commandHandler,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _hearingParticipantService = new HearingParticipantService(commandHandler,eventPublisher);
        }

        /// <summary>
        /// Get participants in a hearing
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param>
        /// <returns>List of participants</returns>
        [HttpGet("{hearingId}/participants")]
        [OpenApiOperation("GetAllParticipantsInHearing")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
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
        [HttpGet("{hearingId}/participants/{participantId}")]
        [OpenApiOperation("GetParticipantInHearing")]
        [ProducesResponseType(typeof(ParticipantResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
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
                var participant = participants.Find(x => x.Id == participantId);

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
        [HttpPost("{hearingId}/participants")]
        [OpenApiOperation("AddParticipantsToHearing")]
        [ProducesResponseType(typeof(List<ParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
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

            var caseTypequery = new GetCaseRolesForCaseTypeQuery(videoHearing.CaseType.Name);
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(caseTypequery);

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
            await _hearingParticipantService
                .PublishEventForNewParticipantsAsync(hearing, participants);

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
        [HttpPost("{hearingId}/updateParticipants")]
        [OpenApiOperation("UpdateHearingParticipants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateHearingParticipants(Guid hearingId, [FromBody] UpdateHearingParticipantsRequest request)
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

            var caseTypeQuery = new GetCaseRolesForCaseTypeQuery(videoHearing.CaseType.Name);
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseTypeQuery, CaseType>(caseTypeQuery);

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
                existingParticipantDetail.Person.ContactEmail = existingParticipantRequest.ContactEmail ?? existingParticipant.Person.ContactEmail;
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
            await _hearingParticipantService
                .PublishEventForUpdateParticipantsAsync(hearing, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

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
        [HttpDelete("{hearingId}/participants/{participantId}")]
        [OpenApiOperation("RemoveParticipantFromHearing")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
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

            var participant = participants.Find(x => x.Id == participantId);
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
        [HttpPut("{hearingId}/participants/{participantId}")]
        [OpenApiOperation("UpdateParticipantDetails")]
        [ProducesResponseType(typeof(ParticipantResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateParticipantDetails(Guid hearingId, Guid participantId,
            [FromBody] UpdateParticipantRequest request)
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
                var repValidationResult =
                    await _hearingParticipantService.ValidateRepresentativeInformationAsync(request);
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
            
            var updatedParticipant =
                await _hearingParticipantService.UpdateParticipantAndPublishEventAsync(videoHearing,
                    updateParticipantCommand);
            
            var response = new ParticipantToResponseMapper().MapParticipantToResponse(updatedParticipant);

            return Ok(response);
        }
        

        private static List<ParticipantResponse> CreateParticipantResponseList(IEnumerable<Participant> participants)
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
