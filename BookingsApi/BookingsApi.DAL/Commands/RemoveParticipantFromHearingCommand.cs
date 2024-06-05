using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

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
                .Include(h => h.Endpoints)
                    .ThenInclude(x => x.EndpointParticipants)
                    .ThenInclude(x => x.Participant)
                    .ThenInclude(x => x.Person)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
                    
            if (hearing == null)
                throw new HearingNotFoundException(command.HearingId);
            
            var participant = hearing.GetParticipants().SingleOrDefault( e => e.Id == command.Participant.Id);
    
            if (participant == null)
                throw new ParticipantNotFoundException(command.Participant.Id);
            
            if(participant.EndpointLinkedParticipants?.Any() ?? false)
                foreach (var endpoint in hearing.Endpoints)
                    endpoint.RemoveLinkedParticipant(participant);
            
            //Save removal FK participants first
            await _context.SaveChangesAsync();
            
            hearing.RemoveParticipant(participant);
            hearing.UpdateBookingStatusJudgeRequirement();
            await _context.SaveChangesAsync();
        }
    }
}