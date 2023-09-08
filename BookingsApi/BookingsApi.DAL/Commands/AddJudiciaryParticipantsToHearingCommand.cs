using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;

namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryParticipantsToHearingCommand : ICommand
    {
        public AddJudiciaryParticipantsToHearingCommand(Guid hearingId, List<NewJudiciaryParticipant> participants)
        {
            HearingId = hearingId;
            Participants = participants;
        }
        
        public IList<NewJudiciaryParticipant> Participants { get; set; }
        public Guid HearingId { get; }
    }

    public class AddJudiciaryParticipantsToHearingCommandHandler : ICommandHandler<AddJudiciaryParticipantsToHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public AddJudiciaryParticipantsToHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(AddJudiciaryParticipantsToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            foreach (var participant in command.Participants)
            {
                await _hearingService.AddJudiciaryParticipantToVideoHearing(hearing, participant);
            }
  
            await _context.SaveChangesAsync();
        }
    }
}
