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

        public HearingParticipantsControllerV2(IQueryHandler queryHandler,
            ICommandHandler commandHandler,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _hearingParticipantService = new HearingParticipantService(commandHandler, eventPublisher);
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
                return NotFound();
            }
            
            var caseTypeQuery = new GetCaseRolesForCaseServiceQuery(videoHearing.CaseType.ServiceId);
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(caseTypeQuery);

            var dataValidationResult = await new AddParticipantsToHearingRequestRefDataValidationV2(caseType).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }

            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            var participants = request.Participants
                .Select(x => ParticipantRequestV2ToNewParticipantMapper.Map(x, videoHearing.CaseType, hearingRoles)).ToList();
            var linkedParticipants =
                LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);
            
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

            var response = CreateParticipantResponseV2List(addedParticipants.ToList());

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
            var result = await new UpdateHearingParticipantsRequestInputValidationV2().ValidateAsync(request);
            if (!result.IsValid)
            {
                ModelState.AddFluentValidationErrors(result.Errors);
                return ValidationProblem(ModelState);
            }

            var query = new GetHearingByIdQuery(hearingId);
            var videoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

            if (videoHearing == null)
            {
                return NotFound();
            }
            
            var caseTypeQuery = new GetCaseRolesForCaseServiceQuery(videoHearing.CaseType.ServiceId);
            var hearingRoles = await _queryHandler.Handle<GetHearingRolesQuery, List<HearingRole>>(new GetHearingRolesQuery());
            var caseType = await _queryHandler.Handle<GetCaseRolesForCaseServiceQuery, CaseType>(caseTypeQuery);
            var dataValidationResult = await new UpdateHearingParticipantsRequestRefDataValidationV2(caseType, hearingRoles).ValidateAsync(request);
            if (!dataValidationResult.IsValid)
            {
                ModelState.AddFluentValidationErrors(dataValidationResult.Errors);
                return ValidationProblem(ModelState);
            }
            
            var newParticipants = request.NewParticipants
                .Select(x => ParticipantRequestV2ToNewParticipantMapper.Map(x, videoHearing.CaseType, hearingRoles)).ToList();

            var existingParticipants = videoHearing.Participants
                .Where(x => request.ExistingParticipants.Select(ep => ep.ParticipantId).Contains(x.Id)).ToList();

            var existingParticipantDetails = UpdateExistingParticipantDetailsFromRequest(request, existingParticipants);

            var linkedParticipants =
                LinkedParticipantRequestV2ToLinkedParticipantDtoMapper.MapToDto(request.LinkedParticipants);

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
            await _hearingParticipantService.PublishEventForUpdateParticipantsAsync(hearing, existingParticipantDetails, newParticipants, request.RemovedParticipantIds, linkedParticipants);

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
                existingParticipantDetail.Person.ContactEmail =
                    existingParticipantRequest.ContactEmail ?? existingParticipant.Person.ContactEmail;
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
