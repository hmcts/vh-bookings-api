using BookingsApi.DAL.Dtos;
using BookingsApi.Common.Helpers;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
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
        public ScreeningDto Screening { get; set; }
        public string ExternalParticipantId { get; set; }
        public string MeasuresExternalId { get; set; }
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
        private readonly IHearingService _hearingService;

        public AddEndPointToHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(AddEndPointToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(x=> x.Participants).ThenInclude(x=> x.InterpreterLanguage)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
                .Include(x=> x.Participants)
                // keep the following includes for the screening entities - cannot auto include due to cyclic dependency
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            var languages = await _context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            var dto = command.Endpoint;
            var defenceAdvocate = DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(dto.ContactEmail, hearing.GetParticipants());
            var endpoint = new Endpoint(dto.ExternalParticipantId, dto.DisplayName, dto.Sip, dto.Pin, defenceAdvocate);
            endpoint.UpdateExternalIds(dto.ExternalParticipantId, dto.MeasuresExternalId);
            var language = languages.GetLanguage(dto.LanguageCode, "Endpoint");
            endpoint.UpdateLanguagePreferences(language, dto.OtherLanguage);
            hearing.AddEndpoint(endpoint);

            // have to get the object added since a new instance is created
            var addedEndpoint = hearing.GetEndpoints().First(x => x.Sip.Equals(dto.Sip));
            _hearingService.UpdateEndpointScreeningRequirement(hearing, addedEndpoint, command.Endpoint.Screening);
            await _context.SaveChangesAsync();
        }
    }
}
