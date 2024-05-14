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
    public class JudiciaryParticipantsController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IJudiciaryParticipantService _judiciaryParticipantService;
        
        public JudiciaryParticipantsController(
            IQueryHandler queryHandler,
            IJudiciaryParticipantService judiciaryParticipantService)
        {
            _queryHandler = queryHandler;
            _judiciaryParticipantService = judiciaryParticipantService;
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

            IList<JudiciaryParticipant> addedParticipants = new List<JudiciaryParticipant>();
            
            try
            {
                addedParticipants = await _judiciaryParticipantService.AddJudiciaryParticipants(participants, hearingId);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (JudiciaryPersonNotFoundException exception)
            {
                return NotFound(exception.Message);
            }

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
            try
            {
                await _judiciaryParticipantService.RemoveJudiciaryParticipant(personalCode, hearingId);
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

            JudiciaryParticipant updatedParticipant;

            try
            {
                updatedParticipant = await _judiciaryParticipantService.UpdateJudiciaryParticipant(participant, hearingId);
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

            JudiciaryParticipant newJudge;
            
            try
            {
                newJudge = await _judiciaryParticipantService.ReassignJudiciaryJudge(hearing.Id, newJudiciaryJudge);
            }
            catch (HearingNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            catch (JudiciaryPersonNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
            
            var mapper = new JudiciaryParticipantToResponseMapper();
            var response = mapper.MapJudiciaryParticipantToResponse(newJudge);
            
            return Ok(response);
        }
    }
}
