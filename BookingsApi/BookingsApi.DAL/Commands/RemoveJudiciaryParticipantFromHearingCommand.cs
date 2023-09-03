namespace BookingsApi.DAL.Commands;

public class RemoveJudiciaryParticipantFromHearingCommand : ICommand
{
    public Guid HearingId { get; set; }
    public JudiciaryParticipant JudiciaryParticipant { get; set; }
    
    public RemoveJudiciaryParticipantFromHearingCommand(Guid hearingId, JudiciaryParticipant judiciaryParticipant)
    {
        HearingId = hearingId;
        JudiciaryParticipant = judiciaryParticipant;
    }
}

public class RemoveJudiciaryParticipantFromHearingCommandHandler : ICommandHandler<RemoveJudiciaryParticipantFromHearingCommand>
{
    private readonly BookingsDbContext _context;

    public RemoveJudiciaryParticipantFromHearingCommandHandler(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveJudiciaryParticipantFromHearingCommand judiciaryParticipantFromHearingCommand)
    {
        var hearing = await _context.VideoHearings
            .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
            .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
            .SingleOrDefaultAsync(x => x.Id == judiciaryParticipantFromHearingCommand.HearingId);

        if (hearing == null)
        {
            throw new HearingNotFoundException(judiciaryParticipantFromHearingCommand.HearingId);
        }

        hearing.RemoveJudiciaryParticipant(judiciaryParticipantFromHearingCommand.JudiciaryParticipant);
        await _context.SaveChangesAsync();
    }
}