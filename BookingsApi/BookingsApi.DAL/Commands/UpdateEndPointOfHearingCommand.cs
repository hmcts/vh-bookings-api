using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.DAL.Commands
{
    public class UpdateEndPointOfHearingCommand : ICommand
    {
        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName, Participant defenceAdvocate,
            string languageCode, string otherLanguage)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
            DefenceAdvocate = defenceAdvocate;
            LanguageCode = languageCode;
            OtherLanguage = otherLanguage;
        }

        public Guid HearingId { get; }
        public Guid EndpointId { get;  }
        public string DisplayName { get;  }
        public Participant DefenceAdvocate { get; }
        public string LanguageCode { get; set; }
        public string OtherLanguage { get; set; }
}

    public class UpdateEndPointOfHearingCommandHandler : ICommandHandler<UpdateEndPointOfHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateEndPointOfHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateEndPointOfHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var endpoint = hearing.Endpoints.SingleOrDefault(e => e.Id == command.EndpointId);
            if (endpoint == null)
            {
                throw new EndPointNotFoundException(command.EndpointId);
            }

            if (!string.IsNullOrWhiteSpace(endpoint.DisplayName)) endpoint.UpdateDisplayName(command.DisplayName);
            if (command.DefenceAdvocate != null)
            {
                var defenceAdvocate = hearing.GetParticipants().Single(x => x.Id == command.DefenceAdvocate.Id);
                endpoint.AssignDefenceAdvocate(defenceAdvocate);
            }
            else
            {
                endpoint.AssignDefenceAdvocate(null);
            }
            
            var languages = await _context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            endpoint.UpdateLanguagePreferences(GetLanguage(languages, command.LanguageCode), command.OtherLanguage);
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
