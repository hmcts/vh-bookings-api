using BookingsApi.DAL.Helper;
using BookingsApi.Domain.Extensions;

namespace BookingsApi.DAL.Commands
{
    public class NewEndpoint
    {
        public string DisplayName { get; set; }
        public string Sip { get; set; }
        public string Pin { get; set; }
        public string ContactEmail { get; set; } 
        public string LanguageCode { get; set; }
        public string OtherLanguage { get; set; }
    }
    
    public class AddEndPointToHearingCommand : ICommand
    {
        public AddEndPointToHearingCommand(Guid hearingId, NewEndpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = endpoint;
        }

        public Guid HearingId { get; }
        public NewEndpoint Endpoint { get; }
    }

    public class AddEndPointToHearingCommandHandler : ICommandHandler<AddEndPointToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddEndPointToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddEndPointToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(x=> x.Participants).ThenInclude(x=> x.InterpreterLanguage)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            var languages = await _context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            var dto = command.Endpoint;
            var defenceAdvocate = DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(dto.ContactEmail, hearing.GetParticipants());
            var endpoint = new Endpoint(dto.DisplayName, dto.Sip, dto.Pin, defenceAdvocate);
            var language = languages.GetLanguage(dto.LanguageCode, "Endpoint");
            endpoint.UpdateLanguagePreferences(language, dto.OtherLanguage);
            hearing.AddEndpoint(endpoint);
            await _context.SaveChangesAsync();
        }
    }
}
