using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain.JudiciaryParticipants;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V1;
using BookingsApi.Validations.V1;

namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class JudiciaryParticipantsController : Controller
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IEventPublisher _eventPublisher;
        
        public JudiciaryParticipantsController(
            IQueryHandler queryHandler, 
            ICommandHandler commandHandler,
            IEventPublisher eventPublisher,
            IHearingParticipantService hearingParticipantService)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _hearingParticipantService = hearingParticipantService;
        }
        
        /// <summary>
        /// Add judiciary participants to a hearing
        /// </summary>
        /// <param name="hearingId">The id of the hearing</param>
        /// <param name="request"></param>
        [HttpPost("{hearingId}/joh")]
        [OpenApiOperation("AddJudiciaryParticipantsToHearing")]
        [ProducesResponseType(typeof(List<JudiciaryParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddJudiciaryParticipantsToHearing(Guid hearingId, [FromBody] List<JudiciaryParticipantRequest> request)
        {
            var validation = await new AddJudiciaryParticipantsToHearingRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }

            var participants = request
                .Select(JudiciaryParticipantRequestToNewJudiciaryParticipantMapper.Map)
                .ToList();

            var command = new AddJudiciaryParticipantsToHearingCommand(hearingId, participants);
            
            try
            {
                await _commandHandler.Handle(command);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (JudiciaryPersonNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await PublishEventsForJudiciaryParticipantsAdded(hearing, participants);

            var addedParticipants = hearing.JudiciaryParticipants
                .Where(x => request.Select(p => p.PersonalCode).Contains(x.JudiciaryPerson.PersonalCode));

            var mapper = new JudiciaryParticipantToResponseMapper();
            var response = addedParticipants.Select(mapper.MapJudiciaryParticipantToResponse).ToList();
            
            return Ok(response);
        }

        [HttpDelete]
        [Route("{hearingId}/joh/{personalCode}")]
        [OpenApiOperation("RemoveJudiciaryParticipantFromHearing")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RemoveJudiciaryParticipantFromHearing(Guid hearingId, string personalCode)
        {
            var command = new RemoveJudiciaryParticipantFromHearingCommand(hearingId, personalCode);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (DomainRuleException exception)
            {
                if (exception.ValidationFailures.Exists(x =>
                        x.Message == DomainRuleErrorMessages.JudiciaryParticipantNotFound))
                {
                    return NotFound(DomainRuleErrorMessages.JudiciaryParticipantNotFound);
                }

                throw;
            }

            // ONLY publish this event when Hearing is set for ready for video
            var videoHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await PublishEventForJudiciaryParticipantRemoved(videoHearing, command.RemovedParticipantId.Value);

            return NoContent();
        }

        /// <summary>
        /// Updates a judiciary participant
        /// </summary>
        /// <param name="hearingId">The id of the hearing</param>
        /// <param name="personalCode">The personal code of the judiciary participant</param>
        /// <param name="request"></param>
        [HttpPatch("{hearingId}/joh/{personalCode}")]
        [OpenApiOperation("UpdateJudiciaryParticipant")]
        [ProducesResponseType(typeof(JudiciaryParticipantResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateJudiciaryParticipant(Guid hearingId, string personalCode, 
            [FromBody] UpdateJudiciaryParticipantRequest request)
        {
            var validation = await new UpdateJudiciaryParticipantRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }

            var participant = UpdateJudiciaryParticipantRequestToUpdatedJudiciaryParticipantMapper.Map(personalCode, request);
            
            var command = new UpdateJudiciaryParticipantCommand(hearingId, participant);

            try
            {
                await _commandHandler.Handle(command);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (DomainRuleException exception)
            {
                if (exception.ValidationFailures.Exists(x =>
                        x.Message == DomainRuleErrorMessages.JudiciaryParticipantNotFound))
                {
                    return NotFound(DomainRuleErrorMessages.JudiciaryParticipantNotFound);
                }
                throw;
            }
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await _hearingParticipantService.PublishEventForUpdateJudiciaryParticipantAsync(hearing, participant);

            var updatedParticipant = hearing.JudiciaryParticipants
                .FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == personalCode);

            var mapper = new JudiciaryParticipantToResponseMapper();
            var response = mapper.MapJudiciaryParticipantToResponse(updatedParticipant);
            
            return Ok(response);
        }

        /// <summary>
        /// Replaces the judiciary participant judge on a hearing 
        /// </summary>
        /// <param name="hearingId">The id of the hearing</param>
        /// <param name="request">The new judiciary participant judge</param>
        [HttpPut("{hearingId}/joh/judge")]
        [OpenApiOperation("ReassignJudiciaryJudge")]
        [ProducesResponseType(typeof(JudiciaryParticipantResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ReassignJudiciaryJudge(Guid hearingId, [FromBody] ReassignJudiciaryJudgeRequest request)
        {
            var validation = await new ReassignJudiciaryJudgeRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }
            
            var newJudiciaryJudge = new NewJudiciaryJudge
            {
                DisplayName = request.DisplayName,
                PersonalCode = request.PersonalCode,
                OptionalContactEmail = request.OptionalContactEmail,
                OptionalContactTelephone = request.OptionalContactTelephone
            };
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));

            if (hearing == null)
            {
                return NotFound(new HearingNotFoundException(hearingId).Message);
            }

            var oldJudge = (JudiciaryParticipant)hearing.GetJudge();
            
            var command = new ReassignJudiciaryJudgeCommand(hearingId, newJudiciaryJudge);
            
            try
            {
                await _commandHandler.Handle(command);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (JudiciaryPersonNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            
            hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            
            var newJudge = (JudiciaryParticipant)hearing.GetJudge();

            await PublishEventsForJudiciaryJudgeReassigned(hearing, oldJudge?.Id, newJudge);
            
            var mapper = new JudiciaryParticipantToResponseMapper();
            var response = mapper.MapJudiciaryParticipantToResponse(newJudge);
            
            return Ok(response);
        }

        private async Task PublishEventsForJudiciaryJudgeReassigned(Hearing hearing, Guid? oldJudgeId, JudiciaryParticipant newJudge)
        {
            if (oldJudgeId == newJudge.Id)
            {
                return;
            }
            
            if (oldJudgeId != null)
            {
                await PublishEventForJudiciaryParticipantRemoved(hearing, oldJudgeId.Value);
            }
            
            await PublishEventsForJudiciaryParticipantsAdded(hearing, new List<NewJudiciaryParticipant>
            {
                new()
                {
                    DisplayName = newJudge.DisplayName,
                    PersonalCode = newJudge.JudiciaryPerson.PersonalCode,
                    HearingRoleCode = newJudge.HearingRoleCode
                }
            });
        }
        
        private async Task PublishEventForJudiciaryParticipantRemoved(Hearing hearing, Guid removedJudiciaryParticipantId)
        {
            if (hearing.Status is BookingStatus.Created or BookingStatus.ConfirmedWithoutJudge)
            {
                await _eventPublisher.PublishAsync(
                    new ParticipantRemovedIntegrationEvent(hearing.Id, removedJudiciaryParticipantId));
            }
        }

        private async Task PublishEventsForJudiciaryParticipantsAdded(Hearing hearing, IEnumerable<NewJudiciaryParticipant> participants)
        {
            await _hearingParticipantService.PublishEventForNewJudiciaryParticipantsAsync(hearing, participants);
        }
    }
}
