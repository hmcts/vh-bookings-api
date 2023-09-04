using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
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
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
            _eventPublisher = eventPublisher;
            _hearingParticipantService = new HearingParticipantService(commandHandler, eventPublisher);
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
            catch (DomainRuleException exception)
            {
                ModelState.AddDomainRuleErrors(exception.ValidationFailures);
                return ValidationProblem(ModelState);
            }
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            await _hearingParticipantService.PublishEventForNewJudiciaryParticipantsAsync(hearing, participants);

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

                ModelState.AddDomainRuleErrors(exception.ValidationFailures);
                return ValidationProblem(ModelState);
            }

            // ONLY publish this event when Hearing is set for ready for video
            var videoHearing =
                await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));
            if (videoHearing.Status == BookingStatus.Created)
            {
                await _eventPublisher.PublishAsync(
                    new ParticipantRemovedIntegrationEvent(hearingId, command.RemovedParticipantId.Value));
            }

            return NoContent();
        }
    }
}