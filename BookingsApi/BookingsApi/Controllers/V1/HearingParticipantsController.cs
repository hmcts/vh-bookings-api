namespace BookingsApi.Controllers.V1
{
    [Produces("application/json")]
    [Route("hearings")]
    [ApiVersion("1.0")]
    [ApiController]
    public class HearingParticipantsController(
        IQueryHandler queryHandler,
        IHearingParticipantService hearingParticipantService)
        : ControllerBase
    {
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
                return ValidationProblem(ModelState);
            }

            if (participantId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(participantId), $"Please provide a valid {nameof(participantId)}");
                return ValidationProblem(ModelState);
            }

            var query = new GetParticipantsInHearingQuery(hearingId);
            var participants = await queryHandler.Handle<GetParticipantsInHearingQuery, List<Participant>>(query);
            

            var participant = participants.Find(x => x.Id == participantId);
            if (participant == null)
            {
                return NotFound();
            }

            var getHearingByIdQuery = new GetHearingByIdQuery(hearingId);
            var videoHearing = await queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
            await hearingParticipantService.RemoveParticipantAndPublishEventAsync(videoHearing, participant);
            return NoContent();
        }
    }
}
