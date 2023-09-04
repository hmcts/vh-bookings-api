namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryParticipantCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public string PersonalCode { get; set; }
        public string NewDisplayName { get; set; }
        public JudiciaryParticipantHearingRoleCode NewHearingRoleCode { get; set; }
        
        public UpdateJudiciaryParticipantCommand(Guid hearingId, string personalCode, string newDisplayName, 
            JudiciaryParticipantHearingRoleCode newHearingRoleCode)
        {
            HearingId = hearingId;
            PersonalCode = personalCode;
            NewDisplayName = newDisplayName;
            NewHearingRoleCode = newHearingRoleCode;
        }
    }

    public class UpdateJudiciaryParticipantCommandHandler : ICommandHandler<UpdateJudiciaryParticipantCommand>
    {
        private readonly BookingsDbContext _context;
        
        public UpdateJudiciaryParticipantCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateJudiciaryParticipantCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            hearing.UpdateJudiciaryParticipantByPersonalCode(command.PersonalCode, command.NewDisplayName, command.NewHearingRoleCode);
            
            await _context.SaveChangesAsync();
        }
    }
}
