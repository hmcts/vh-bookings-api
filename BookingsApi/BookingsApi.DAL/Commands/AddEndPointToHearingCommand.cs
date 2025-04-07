using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace BookingsApi.DAL.Commands;

public class NewEndpoint
{
    public string DisplayName { get; set; }
    public string Sip { get; set; }
    public string Pin { get; set; }
    public List<string> LinkedParticipantEmails { get; set; } 
    public string LanguageCode { get; set; }
    public string OtherLanguage { get; set; }
    public ScreeningDto Screening { get; set; }
    public string ExternalParticipantId { get; set; }
    public string MeasuresExternalId { get; set; }
}
    
public class AddEndPointToHearingCommand(Guid hearingId, NewEndpoint endpoint) : ICommand
{
    public Guid HearingId { get; } = hearingId;
    public NewEndpoint Endpoint { get; } = endpoint;
}

public class AddEndPointToHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
    : ICommandHandler<AddEndPointToHearingCommand>
{
    public async Task Handle(AddEndPointToHearingCommand command)
    {
        var hearing = await context.VideoHearings
            .Include(h => h.Participants).ThenInclude(x => x.Person)
            .Include(x => x.Participants).ThenInclude(x => x.InterpreterLanguage)
            .Include(h => h.Endpoints).ThenInclude(x => x.ParticipantsLinked).ThenInclude(x => x.Person)
            .Include(x => x.Endpoints).ThenInclude(x => x.InterpreterLanguage)
            .Include(x => x.Participants)
            // keep the following includes for the screening entities - cannot auto include due to cyclic dependency
            .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x => x.ScreeningEntities)
            .ThenInclude(x => x.Participant)
            .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x => x.ScreeningEntities)
            .ThenInclude(x => x.Endpoint)
            .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x => x.ScreeningEntities)
            .ThenInclude(x => x.Participant)
            .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x => x.ScreeningEntities)
            .ThenInclude(x => x.Endpoint)
            .SingleOrDefaultAsync(x => x.Id == command.HearingId);

        if (hearing == null)
        {
            throw new HearingNotFoundException(command.HearingId);
        }

        var languages = await context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
        var dto = command.Endpoint;
        var endpoint = new Endpoint(dto.ExternalParticipantId, dto.DisplayName, dto.Sip, dto.Pin);

        LinkParticipants(dto, hearing, endpoint);

        endpoint.UpdateExternalIds(dto.ExternalParticipantId, dto.MeasuresExternalId);
        var language = languages.GetLanguage(dto.LanguageCode, "Endpoint");
        endpoint.UpdateLanguagePreferences(language, dto.OtherLanguage);
        hearing.AddEndpoint(endpoint);

        // have to get the object added since a new instance is created
        var addedEndpoint = hearing.GetEndpoints().First(x => x.Sip.Equals(dto.Sip));
        hearingService.UpdateEndpointScreeningRequirement(hearing, addedEndpoint, command.Endpoint.Screening);
        await context.SaveChangesAsync();
    }

    private static void LinkParticipants(NewEndpoint dto, VideoHearing hearing, Endpoint endpoint)
    {
        if (!dto.LinkedParticipantEmails.IsNullOrEmpty())
        {
            var participantsLinked = EndpointParticipantHelper.CheckAndReturnParticipantsLinkedToEndpoint(dto.LinkedParticipantEmails, hearing.GetParticipants().ToList());
            foreach (var participant in participantsLinked)
                endpoint.AddLinkedParticipant(participant);
        }
    }
}