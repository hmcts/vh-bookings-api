using BookingsApi.Domain.Extensions;

namespace BookingsApi.DAL.Commands
{
    public class UpdatedJudiciaryParticipant
    {
        public string DisplayName { get; set; }
        public string PersonalCode { get; set; }
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }
        public string InterpreterLanguageCode { get; set; }
        public string OtherLanguage { get; set; }
    }
    
    public class UpdateJudiciaryParticipantCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public UpdatedJudiciaryParticipant UpdatedJudiciaryParticipant { get; set; }

        public UpdateJudiciaryParticipantCommand(Guid hearingId, UpdatedJudiciaryParticipant updatedJudiciaryParticipant)
        {
            HearingId = hearingId;
            UpdatedJudiciaryParticipant = updatedJudiciaryParticipant;
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
                .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.InterpreterLanguage)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var languages = await _context.InterpreterLanguages.ToListAsync();
            var language = languages.GetLanguage(command.UpdatedJudiciaryParticipant.InterpreterLanguageCode, "JudiciaryParticipant");
            hearing.UpdateJudiciaryParticipantByPersonalCode(command.UpdatedJudiciaryParticipant.PersonalCode,
                command.UpdatedJudiciaryParticipant.DisplayName, command.UpdatedJudiciaryParticipant.HearingRoleCode,
                language, command.UpdatedJudiciaryParticipant.OtherLanguage);
            await _context.SaveChangesAsync();
        }
    }
}
