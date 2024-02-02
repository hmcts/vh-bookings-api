using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;

namespace BookingsApi.Controllers.V2
{
    [Produces("application/json")]
    [Route(template:"v{version:apiVersion}/hearings")]
    [ApiVersion("2.0")]
    [ApiController]
    public class HearingParticipantsControllerV2 : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IUpdateHearingService _updateHearingService;

        public HearingParticipantsControllerV2(IQueryHandler queryHandler,
            ICommandHandler commandHandler,
            IEventPublisher eventPublisher,
            IHearingParticipantService hearingParticipantService,
            IUpdateHearingService updateHearingService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hearingParticipantService = hearingParticipantService;
            _updateHearingService = updateHearingService;
        }
        
        /// <summary>
        /// Add participant(s) to a hearing
        /// NOT USED BY ADMIN WEB
        /// </summary>
        /// <param name="hearingId">The Id of the hearing</param> 
        /// <param name="request">The participant information to add</param>
        /// <returns>The participant</returns>
        [HttpPost("{hearingId}/participants")]
        [OpenApiOperation("AddParticipantsToHearing")]
        [ProducesResponseType(typeof(List<ParticipantResponseV2>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> AddParticipantsToHearing(Guid hearingId, [FromBody] AddParticipantsToHearingRequestV2 request)
        {
            var result = await new AddParticipantsToHearingRequestInputValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound("Video hearing not found");
            }
            
            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());

            var dataValidationResult = await new AddParticipantsToHearingRequestRefDataValidationV2(hearingRoles).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var participants = request.Participants
                .Select(x => ParticipantRequestV2ToNewParticipantMapper.Map(x, hearingRoles)).ToList();
            var linkedParticipants =
                LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);
            
            var command = new AddParticipantsToVideoHearingCommand(hearingId, participants, linkedParticipants);
            
            await _commandHandler.Handle(command);

            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);
            await _hearingParticipantService
                .PublishEventForNewParticipantsAsync(hearing, participants);

            var addedParticipants = hearing.Participants.Where(x => request.Participants.Select(p => p.ContactEmail).Contains(x.Person.ContactEmail));

            var response = CreateParticipantResponseV2List(addedParticipants.ToList());

            return Ok(response);
        }

        /// <summary>
        /// Update participant details
        /// </summary>
        /// <param name="hearingId">Id of hearing to look up</param>
        /// <param name="participantId">Id of participant to remove</param>
        /// <param name="request">The participant information to add</param>
        /// <returns></returns>
        [HttpPatch("{hearingId}/participants/{participantId}")]
        [OpenApiOperation("UpdateParticipantDetailsV2")]
        [ProducesResponseType(typeof(ParticipantResponseV2),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateParticipantDetailsV2(Guid hearingId, Guid participantId, [FromBody]UpdateParticipantRequestV2 request)
        {
            var requestValidationResult = await new UpdateParticipantRequestValidationV2().ValidateAsync(request);
            if (!requestValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(requestValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

            if (videoHearing == null)
            {
                return NotFound($"Video hearing {hearingId} not found");
            }
            
            var participant = videoHearing.GetParticipants().SingleOrDefault(x => x.Id.Equals(participantId));

            if (participant == null)
            {
                return NotFound($"Participant {participantId} not found for hearing {hearingId}");
            }

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                var repValidationResult = await _hearingParticipantService.ValidateRepresentativeInformationAsync(request);
                if (!repValidationResult.IsValid)
                {
                    ModelState.AddFluentValidationErrors(repValidationResult.Errors);
                    return ValidationProblem(ModelState);
                }
            }

            var representative = new RepresentativeInformation
            {
                Representee = request.Representee
            };

            var linkedParticipants =
                LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);

            var additionalInformation = new AdditionalInformation(request.FirstName, request.LastName)
            {
                MiddleNames = request.MiddleNames
            };
            
            var updateParticipantCommand = new UpdateParticipantCommand(hearingId, participantId, request.Title,
                request.DisplayName, request.TelephoneNumber, request.OrganisationName, representative, linkedParticipants,
                additionalInformation: additionalInformation);

            var updatedParticipant = await _hearingParticipantService.UpdateParticipantAndPublishEventAsync(videoHearing, updateParticipantCommand);
            
            var response = new ParticipantToResponseV2Mapper().MapParticipantToResponse(updatedParticipant);
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
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateHearingParticipants(Guid hearingId, [FromBody] UpdateHearingParticipantsRequestV2 request)
        {
            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            var validationResult = await _updateHearingService.ValidateUpdateParticipantsV2(request, hearingRoles);
            if (!validationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(validationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }

            var hearing = await _updateHearingService.UpdateParticipantsV2(request, videoHearing, hearingRoles);

            var upsertedParticipants = hearing.Participants.Where(x => request.NewParticipants.Select(p => p.ContactEmail).Contains(x.Person.ContactEmail)
                || request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id));

            var response = CreateParticipantResponseV2List(upsertedParticipants.ToList());

            return Ok(response);
        }

        private static List<ExistingParticipantDetails> UpdateExistingParticipantDetailsFromRequest(UpdateHearingParticipantsRequestV2 request,
            List<Participant> existingParticipants)
        {
            var existingParticipantDetails = new List<ExistingParticipantDetails>();

            foreach (var existingParticipantRequest in request.ExistingParticipants)
            {
                var existingParticipant =
                    existingParticipants.SingleOrDefault(ep => ep.Id == existingParticipantRequest.ParticipantId);

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
                    RepresentativeInformation = new RepresentativeInformation {Representee = existingParticipantRequest.Representee},
                    TelephoneNumber = existingParticipantRequest.TelephoneNumber,
                    Title = existingParticipantRequest.Title
                };
                existingParticipantDetail.Person.ContactEmail = existingParticipant.Person.ContactEmail;
                existingParticipantDetails.Add(existingParticipantDetail);
            }

            return existingParticipantDetails;
        }

        private static List<ParticipantResponseV2> CreateParticipantResponseV2List(List<Participant> participants)
        {
            if (participants.Any())
            {
                var mapper = new ParticipantToResponseV2Mapper();
                return participants.Select(x => mapper.MapParticipantToResponse(x)).ToList();
            }

            return new List<ParticipantResponseV2>();
        }
    }
}
