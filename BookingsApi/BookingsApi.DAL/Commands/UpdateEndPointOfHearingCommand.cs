using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands;

public class UpdateEndPointOfHearingCommand(
    Guid hearingId,
    Guid endpointId,
    string displayName,
    IEnumerable<Participant> linkedParticipants,
    string languageCode,
    string otherLanguage,
    ScreeningDto screening,
    string externalReferenceId,
    string measuresExternalId)
    : ICommand
{
    public Guid HearingId { get; } = hearingId;
    public Guid EndpointId { get;  } = endpointId;
    public string DisplayName { get;  } = displayName;
    public List<Participant> LinkedParticipants { get; } = linkedParticipants?.ToList() ?? new List<Participant>();
    public string LanguageCode { get; set; } = languageCode;
    public string OtherLanguage { get; set; } = otherLanguage;
    public string ExternalReferenceId { get; set; } = externalReferenceId;
    public string MeasuresExternalId { get; set; } = measuresExternalId;
    public ScreeningDto Screening { get; } = screening;

    /// <summary>
    /// The updated endpoint entity
    /// </summary>
    public Endpoint UpdatedEndpoint { get; set; }
}

public class UpdateEndPointOfHearingCommandHandler(BookingsDbContext context, IHearingService hearingService) : ICommandHandler<UpdateEndPointOfHearingCommand>
{
    public async Task Handle(UpdateEndPointOfHearingCommand command)
    {
        var hearing = await context.VideoHearings
            .Include(h => h.Participants).ThenInclude(x => x.Person)
            .Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked).ThenInclude(p => p.Person)
            .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
            // keep the following includes for the screening entities - cannot auto include due to cyclic dependency
            .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
            .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
            .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
            .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)

            .SingleOrDefaultAsync(x => x.Id == command.HearingId);

        if (hearing == null)
        {
            throw new HearingNotFoundException(command.HearingId);
        }

        var endpoint = hearing.Endpoints.SingleOrDefault(e => e.Id == command.EndpointId);
        if (endpoint == null)
        {
            throw new EndPointNotFoundException(command.EndpointId);
        }

        var originalUpdatedDate = endpoint.UpdatedDate;

        if (!string.IsNullOrWhiteSpace(endpoint.DisplayName)) endpoint.UpdateDisplayName(command.DisplayName);
            
        UpdateEndpointParticipants(command, endpoint, hearing);
            
        var languages = await context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
        var language = languages.GetLanguage(command.LanguageCode, "Endpoint");
        endpoint.UpdateLanguagePreferences(language, command.OtherLanguage);
            
        hearingService.UpdateEndpointScreeningRequirement(hearing, endpoint, command.Screening);
        endpoint.UpdateExternalIds(command.ExternalReferenceId, command.MeasuresExternalId);

        var endpointHasChanged = endpoint.UpdatedDate != originalUpdatedDate;
        if (!endpointHasChanged) return;
            
        await context.SaveChangesAsync();

        command.UpdatedEndpoint = endpoint;
    }

    private static void UpdateEndpointParticipants(UpdateEndPointOfHearingCommand command, Endpoint endpoint, VideoHearing hearing)
    {
        var currentLinkedIds = endpoint.ParticipantsLinked.Select(p => p.Id).ToList();
        var newLinkedIds = command.LinkedParticipants.Select(p => p.Id).ToList();

        // Remove participants that are no longer linked
        foreach (var participant in endpoint.ParticipantsLinked.Where(p => !newLinkedIds.Contains(p.Id)).ToList()) 
            endpoint.RemoveLinkedParticipant(participant);

        // Add new participants
        foreach (var linkedId in newLinkedIds.Except(currentLinkedIds))
        {
            var participant = hearing.GetParticipants().SingleOrDefault(p => p.Id == linkedId) ?? throw new ParticipantNotFoundException(linkedId);
            endpoint.AddLinkedParticipant(participant);
        }
    }
}