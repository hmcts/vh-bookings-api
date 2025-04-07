using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands
{
    public class RemoveParticipantFromHearingCommand(Guid hearingId, Participant participant) : ICommand
    {
        public Guid HearingId { get; set; } = hearingId;
        public Participant Participant { get; set; } = participant;
    }
    
    public class RemoveParticipantFromHearingCommandHandler(BookingsDbContext context)
        : ICommandHandler<RemoveParticipantFromHearingCommand>
    {
        public async Task Handle(RemoveParticipantFromHearingCommand command)
        {
            var hearing = await context.VideoHearings
                .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
                .Include(x => x.Participants).ThenInclude(x => x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(h => h.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Endpoints).ThenInclude(x => x.ParticipantsLinked).ThenInclude(p => p.Person)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
                throw new HearingNotFoundException(command.HearingId);
            
            hearing.RemoveParticipant(command.Participant);
            hearing.UpdateBookingStatusJudgeRequirement();
            await context.SaveChangesAsync();
        }
    }
}