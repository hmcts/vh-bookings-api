
using BookingsApi.Domain.Dtos;

namespace BookingsApi.DAL.Commands;

public class AddEndPointToHearingCommand : ICommand
{
    public AddEndPointToHearingCommand(Guid hearingId, NewEndpointDto newEndpointDto)
    {
        HearingId = hearingId;
        NewEndpointDto = newEndpointDto;
    }

    public Guid HearingId { get; }
    public NewEndpointDto NewEndpointDto { get; }
}

public class AddEndPointToHearingCommandHandler : ICommandHandler<AddEndPointToHearingCommand>
{
    private readonly BookingsDbContext _context;

    public AddEndPointToHearingCommandHandler(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(AddEndPointToHearingCommand command)
    {
        var hearing = await _context.VideoHearings
            .Include(h => h.Participants).ThenInclude(x => x.Person)
            .Include(h => h.Endpoints)
            .ThenInclude(e => e.EndpointParticipants)
            .ThenInclude(x => x.Participant)
            .ThenInclude(x => x.Person)
            .SingleOrDefaultAsync(x => x.Id == command.HearingId);

        if (hearing == null)
            throw new HearingNotFoundException(command.HearingId);
            
        var dto = command.NewEndpointDto;
        var endpoint = new Endpoint(dto.DisplayName, dto.Sip, dto.Pin);
        foreach (var endpointParticipant in dto.EndpointParticipants)
        {
            var participant = hearing.GetParticipants().SingleOrDefault(x => x.Person.ContactEmail == endpointParticipant.ContactEmail);
            endpoint.LinkParticipantToEndpoint(participant, endpointParticipant.Type);
        }
        hearing.AddEndpoint(endpoint);
        await _context.SaveChangesAsync();
    }
}