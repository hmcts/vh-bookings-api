using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations.V2;

namespace BookingsApi.Controllers.V2
{
    [Produces("application/json")]
    [Route(template: "v{version:apiVersion}/hearings")]
    [ApiVersion("2.0")]
    [ApiController]
    public class JudiciaryParticipantsController(
        IQueryHandler queryHandler,
        IJudiciaryParticipantService judiciaryParticipantService)
        : ControllerBase
    {
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
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> AddJudiciaryParticipantsToHearing(Guid hearingId,
            [FromBody] List<JudiciaryParticipantRequest> request)
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

            var addedParticipants = await judiciaryParticipantService.AddJudiciaryParticipants(participants, hearingId);

            var mapper = new JudiciaryParticipantToResponseMapper();
            var response = addedParticipants.Select(mapper.MapJudiciaryParticipantToResponse).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Remove a judiciary participant from a hearing
        /// </summary>
        /// <param name="hearingId">The hearing ID</param>
        /// <param name="personalCode">The personal code of the judiciary participant</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{hearingId}/joh/{personalCode}")]
        [OpenApiOperation("RemoveJudiciaryParticipantFromHearing")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int) HttpStatusCode.BadRequest)]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> RemoveJudiciaryParticipantFromHearing(Guid hearingId, string personalCode)
        {
            try
            {
                await judiciaryParticipantService.RemoveJudiciaryParticipant(personalCode, hearingId);
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
                updatedParticipant = await judiciaryParticipantService.UpdateJudiciaryParticipant(participant, hearingId);
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
                OptionalContactTelephone = request.OptionalContactTelephone,
                InterpreterLanguageCode = request.InterpreterLanguageCode,
                OtherLanguage = request.OtherLanguage
            };
            
            var hearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(hearingId));

            if (hearing == null)
            {
                throw new HearingNotFoundException(hearingId);
            }

            var newJudge = await judiciaryParticipantService.ReassignJudiciaryJudge(hearing.Id, newJudiciaryJudge);
            var mapper = new JudiciaryParticipantToResponseMapper();
            var response = mapper.MapJudiciaryParticipantToResponse(newJudge);
            
            return Ok(response);
        }
    }
}
