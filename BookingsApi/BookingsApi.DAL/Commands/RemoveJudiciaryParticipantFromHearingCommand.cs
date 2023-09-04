namespace BookingsApi.DAL.Commands;

public class RemoveJudiciaryParticipantFromHearingCommand : ICommand
{
    public Guid HearingId { get; set; }
    public string PersonalCode { get; set; }
    public Guid? RemovedParticipantId { get; set; }
    
    public RemoveJudiciaryParticipantFromHearingCommand(Guid hearingId, string personalCode)
    {
        HearingId = hearingId;
        PersonalCode = personalCode;
    }
}

public class RemoveJudiciaryParticipantFromHearingCommandHandler : ICommandHandler<RemoveJudiciaryParticipantFromHearingCommand>
{
    private readonly BookingsDbContext _context;

    public RemoveJudiciaryParticipantFromHearingCommandHandler(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveJudiciaryParticipantFromHearingCommand command)
    {
        var hearing = await _context.VideoHearings
            .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
            .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
            .SingleOrDefaultAsync(x => x.Id == command.HearingId);

        if (hearing == null)
        {
            throw new HearingNotFoundException(command.HearingId);
        }

        var judiciaryParticipant = hearing.JudiciaryParticipants.FirstOrDefault(x => x.JudiciaryPerson.PersonalCode == command.PersonalCode);
        command.RemovedParticipantId = judiciaryParticipant?.Id;
        
        hearing.RemoveJudiciaryParticipantByPersonalCode(command.PersonalCode);
        await _context.SaveChangesAsync();
    }
}