namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryParticipantToHearingCommand : ICommand
    {
        public AddJudiciaryParticipantToHearingCommand(string displayName, Guid judiciaryPersonId, 
            HearingRoleCode hearingRoleCode, Guid hearingId)
        {
            DisplayName = displayName;
            JudiciaryPersonId = judiciaryPersonId;
            HearingRoleCode = hearingRoleCode;
            HearingId = hearingId;
        }
        
        public string DisplayName { get; }
        public Guid JudiciaryPersonId { get; }
        public HearingRoleCode HearingRoleCode { get; }
        public Guid HearingId { get; }
    }

    public class AddJudiciaryParticipantToHearingCommandHandler : ICommandHandler<AddJudiciaryParticipantToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryParticipantToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddJudiciaryParticipantToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            var judiciaryPerson = await _context.JudiciaryPersons
                .SingleOrDefaultAsync(x => x.Id == command.JudiciaryPersonId);

            if (judiciaryPerson == null)
            {
                throw new JudiciaryPersonNotFoundException(command.JudiciaryPersonId);
            }
            
            hearing.AddJudiciaryParticipant(judiciaryPerson, command.DisplayName, command.HearingRoleCode);
            
            await _context.SaveChangesAsync();
        }
    }
}
