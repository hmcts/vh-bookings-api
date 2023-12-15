using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.JudiciaryParticipants;

namespace BookingsApi.DAL.Commands
{
    public class ReassignJudiciaryJudgeCommand : ICommand
    {
        public Guid HearingId { get; }
        public NewJudiciaryJudge NewJudiciaryJudge { get; }
        
        public ReassignJudiciaryJudgeCommand(Guid hearingId, NewJudiciaryJudge newJudiciaryJudge)
        {
            HearingId = hearingId;
            NewJudiciaryJudge = newJudiciaryJudge;
        }
    }

    public class ReassignJudiciaryJudgeCommandHandler : ICommandHandler<ReassignJudiciaryJudgeCommand>
    {
        private readonly BookingsDbContext _context;
        
        public ReassignJudiciaryJudgeCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(ReassignJudiciaryJudgeCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            var judiciaryPerson = await _context.JudiciaryPersons
                .SingleOrDefaultAsync(x => x.PersonalCode == command.NewJudiciaryJudge.PersonalCode);
            
            if (judiciaryPerson == null)
            {
                throw new JudiciaryPersonNotFoundException(command.NewJudiciaryJudge.PersonalCode);
            }
            
            var newJudge = new JudiciaryJudge(command.NewJudiciaryJudge.DisplayName, judiciaryPerson);
            
            hearing.ReassignJudiciaryJudge(newJudge);

            await _context.SaveChangesAsync();
        }
    }
}
