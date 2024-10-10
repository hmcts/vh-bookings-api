using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands
{
    public class UpdateEndPointOfHearingCommand : ICommand
    {
        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName, Participant defenceAdvocate,
            string languageCode, string otherLanguage, ScreeningDto screening)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
            DefenceAdvocate = defenceAdvocate;
            LanguageCode = languageCode;
            OtherLanguage = otherLanguage;
            Screening = screening;
        }

        public Guid HearingId { get; }
        public Guid EndpointId { get;  }
        public string DisplayName { get;  }
        public Participant DefenceAdvocate { get; }
        public string LanguageCode { get; set; }
        public string OtherLanguage { get; set; }
        public ScreeningDto Screening { get; }
        
        /// <summary>
        /// The updated endpoint entity
        /// </summary>
        public Endpoint UpdatedEndpoint { get; set; }
    }

    public class UpdateEndPointOfHearingCommandHandler : ICommandHandler<UpdateEndPointOfHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public UpdateEndPointOfHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(UpdateEndPointOfHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
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
            var language = languages.GetLanguage(command.LanguageCode, "Endpoint");
            endpoint.UpdateLanguagePreferences(language, command.OtherLanguage);
            
            _hearingService.UpdateEndpointScreeningRequirement(hearing, endpoint, command.Screening);
            await _context.SaveChangesAsync();

            command.UpdatedEndpoint = endpoint;
        }
    }
}
