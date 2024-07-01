using BookingsApi.DAL.Dtos;
using BookingsApi.Domain.JudiciaryParticipants;
using BookingsApi.Domain.Validations;

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
            
            var newJudge = new JudiciaryJudge(command.NewJudiciaryJudge.DisplayName, judiciaryPerson, command.NewJudiciaryJudge.OptionalContactEmail);
            var languages = await _context.InterpreterLanguages.Where(x=> x.Live).ToListAsync();
            var interpreterLanguage = GetLanguage(languages, command.NewJudiciaryJudge.InterpreterLanguageCode);
            var otherLanguage = command.NewJudiciaryJudge.OtherLanguage;
            
            hearing.ReassignJudiciaryJudge(newJudge, interpreterLanguage, otherLanguage);

            await _context.SaveChangesAsync();
        }
        
        private InterpreterLanguage GetLanguage(List<InterpreterLanguage> languages, string languageCode)
        {
            if(string.IsNullOrWhiteSpace(languageCode)) return null;
            var language = languages.Find(x=> x.Code == languageCode);

            if (language == null)
            {
                throw new DomainRuleException("Hearing", $"Language code {languageCode} does not exist");
            }
            return language;
        }
    }
}
