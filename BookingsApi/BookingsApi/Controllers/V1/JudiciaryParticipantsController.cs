using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Validations.V1;
using JudiciaryParticipantHearingRoleCode = BookingsApi.Contract.V1.Requests.Enums.JudiciaryParticipantHearingRoleCode;

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
        
        public JudiciaryParticipantsController(
            IQueryHandler queryHandler, 
            ICommandHandler commandHandler)
        {
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }
        
        /// <summary>
        /// Add judiciary participants to a hearing
        /// </summary>
        /// <param name="hearingId">The id of the hearing</param>
        /// <param name="request"></param>
        [HttpPost("{hearingId}/joh")]
        [OpenApiOperation("AddJudiciaryParticipantsToHearing")]
        [ProducesResponseType(typeof(List<JudiciaryParticipantResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddJudiciaryParticipantsToHearing(Guid hearingId, [FromBody] AddJudiciaryParticipantsRequest request)
        {
            var validation = await new AddJudiciaryParticipantsToHearingRequestValidation().ValidateAsync(request);
            if (!validation.IsValid)
            {
                ModelState.AddFluentValidationErrors(validation.Errors);
                return ValidationProblem(ModelState);
            }       
            
            foreach (var participant in request.Participants)
            {
                // TODO move to mapper
                Domain.Enumerations.JudiciaryParticipantHearingRoleCode hearingRoleCode;
                
                switch (participant.HearingRoleCode)
                {
                    case JudiciaryParticipantHearingRoleCode.Judge:
                        hearingRoleCode = Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge;
                        break;
                    case JudiciaryParticipantHearingRoleCode.PanelMember:
                        hearingRoleCode = Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                try
                {
                    var command = new AddJudiciaryParticipantToHearingCommand(participant.DisplayName, participant.PersonalCode, hearingRoleCode, hearingId);

                    // TODO try catch with DomainRuleException
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
            }
            
            var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));

            // TODO move to mapper
            var participants = hearing.JudiciaryParticipants
                .Where(x => request.Participants.Select(p => p.PersonalCode).Contains(x.JudiciaryPerson.PersonalCode))
                .Select(x => new JudiciaryParticipantResponse
                {
                    PersonalCode = x.JudiciaryPerson.PersonalCode,
                    DisplayName = x.DisplayName,
                    HearingRoleCode = MapHearingRoleCode(x.HearingRoleCode)
                })
                .ToList();

            return Ok(participants);
        }

        private static JudiciaryParticipantHearingRoleCode MapHearingRoleCode(Domain.Enumerations.JudiciaryParticipantHearingRoleCode hearingRoleCode)
        {
            return hearingRoleCode switch
            {
                Domain.Enumerations.JudiciaryParticipantHearingRoleCode.Judge => JudiciaryParticipantHearingRoleCode.Judge,
                Domain.Enumerations.JudiciaryParticipantHearingRoleCode.PanelMember => JudiciaryParticipantHearingRoleCode.PanelMember,
                _ => throw new ArgumentOutOfRangeException(nameof(hearingRoleCode), hearingRoleCode, null)
            };
        }
    }
}
