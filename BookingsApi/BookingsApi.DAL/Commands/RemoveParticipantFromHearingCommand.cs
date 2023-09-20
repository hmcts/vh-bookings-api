using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands
{
    public class RemoveParticipantFromHearingCommand : ICommand
    {
        public RemoveParticipantFromHearingCommand(Guid hearingId, Participant participant)
        {
            HearingId = hearingId;
            Participant = participant;
        }

        public Guid HearingId { get; set; }
        public Participant Participant { get; set; }

    }
    
    public class RemoveParticipantFromHearingCommandHandler : ICommandHandler<RemoveParticipantFromHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public RemoveParticipantFromHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveParticipantFromHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(h => h.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
                throw new HearingNotFoundException(command.HearingId);
            
            hearing.RemoveParticipant(command.Participant);
            await _context.SaveChangesAsync();
        }
    }
}