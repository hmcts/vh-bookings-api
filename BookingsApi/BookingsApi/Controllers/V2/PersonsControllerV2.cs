using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V2;
using BookingsApi.Validations;
using BookingsApi.Validations.V2;

namespace BookingsApi.Controllers.V2;


/// <summary>
/// A suite of operations to Persons
/// </summary>
[Produces("application/json")]
[Route(template: "v{version:apiVersion}/persons")]
[ApiVersion("2.0")]
[ApiController]

public class PersonsControllerV2(
    IQueryHandler queryHandler,
    ICommandHandler commandHandler,
    IEventPublisher eventPublisher,
    ILogger<PersonsControllerV2> logger) : ControllerBase
{
    /// <summary>
    /// Find persons with contact email matching a search term.
    /// </summary>
    /// <param name="term">Partial string to match contact email with, case-insensitive.</param>
    /// <returns>Person list</returns>
    [HttpPost]
    [OpenApiOperation("SearchForPersonV2")]
    [ProducesResponseType(typeof(IList<PersonResponseV2>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> PersonBySearchTerm(SearchTermRequestV2 term)
    {
        var query = new GetPersonBySearchTermQuery(term.Term);
        var personList = await queryHandler.Handle<GetPersonBySearchTermQuery, List<Person>>(query);

        var response = personList.Select(PersonToResponseV2Mapper.MapPersonToResponse).OrderBy(o => o.ContactEmail)
            .ToList();
        return Ok(response);
    }

    /// <summary>
    /// Find a person with contact email.
    /// </summary>
    /// <param name="contactEmail">The contact email to match</param>
    /// <returns></returns>
    [HttpGet]
    [OpenApiOperation("SearchForNonJudgePersonsByContactEmailV2")]
    [ProducesResponseType(typeof(PersonResponseV2), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<PersonResponseV2>> SearchForNonJudgePersonsByContactEmailAsync(
        [FromQuery] string contactEmail)
    {
        if (!contactEmail.IsValidEmail())
        {
            ModelState.AddModelError(nameof(contactEmail), $"Please provide a valid {nameof(contactEmail)}");
            return ValidationProblem(ModelState);
        }

        var personQuery = new GetPersonByContactEmailQuery(contactEmail);
        var person = await queryHandler.Handle<GetPersonByContactEmailQuery, Person>(personQuery);
        if (person == null)
        {
            return NotFound($"Person with {contactEmail} does not exist");
        }

        var hearingsQuery = new GetHearingsByUsernameQuery(person.Username);
        var hearings = await queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(hearingsQuery);

        var judicialHearings = hearings.SelectMany(v => v.Participants.Where(p => p.PersonId == person.Id))
            .Any(x => x is Judge);

        if (judicialHearings)
        {
            return Unauthorized("Only searches for non Judge persons are allowed");
        }

        return Ok(PersonToResponseV2Mapper.MapPersonToResponse(person));
    }

    /// <summary>
    /// Update the personal details
    /// </summary>
    /// <param name="personId">The id of the person to update</param>
    /// <param name="payload">Updated details of the person</param>
    /// <returns></returns>
    [HttpPut("{personId}")]
    [OpenApiOperation("UpdatePersonDetailsV2")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<IList<PersonResponseV2>>> UpdatePersonDetails([FromRoute] Guid personId,
        [FromBody] UpdatePersonDetailsRequestV2 payload)
    {
        var validation = await new UpdatePersonDetailsRequestValidationV2().ValidateAsync(payload);
        if (!validation.IsValid)
        {
            ModelState.AddFluentValidationErrors(validation.Errors);
            return ValidationProblem(ModelState);
        }

        var command = new UpdatePersonCommand(personId, payload.FirstName, payload.LastName, payload.Username);
        await commandHandler.Handle(command);

        // get all hearings for user
        var query = new GetHearingsByUsernameQuery(payload.Username);
        var hearings = await queryHandler.Handle<GetHearingsByUsernameQuery, List<VideoHearing>>(query);

        // raise an update event for each hearing to ensure consistency between video and bookings api
        const string anonymisedText = "@hmcts.net";
        var nonAnonymisedParticipants = hearings
            .Where(x => x.Status == BookingStatus.Created &&
                        x.GetCases().Any(c => !c.Name.EndsWith(anonymisedText))).SelectMany(c => c.Participants)
            .Where(p => p.PersonId == personId && !p.DisplayName.EndsWith(anonymisedText)).ToList();
        logger.LogDebug("Updating {Count} non-anonymised participants", nonAnonymisedParticipants.Count);

        foreach (var participant in nonAnonymisedParticipants)
        {
            // map to updated participant event
                await eventPublisher.PublishAsync(
                new ParticipantUpdatedIntegrationEvent(participant.HearingId, participant));
        }

        return Accepted();
    }
}