using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;

namespace BookingsApi.DAL.Commands
{

    public class NewParticipant
    {
        public Person Person { get; set; }
        public CaseRole CaseRole { get; set; }
        public HearingRole HearingRole { get; set; }
        public string Representee { get; set; }
        public string DisplayName { get; set; }
        public string InterpreterLanguageCode { get; set; }
        public string OtherLanguage { get; set; }
    }
    
    public class AddParticipantsToVideoHearingCommand : ICommand
    {
        public AddParticipantsToVideoHearingCommand(Guid hearingId, List<NewParticipant> participants, List<LinkedParticipantDto> linkedParticipants)
        {
            HearingId = hearingId;
            Participants = participants;
            LinkedParticipants = linkedParticipants ?? new List<LinkedParticipantDto>();
        }

        public List<NewParticipant> Participants { get; set; }
        public Guid HearingId { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }
    }
    
    public class AddParticipantsToVideoHearingCommandHandler : ICommandHandler<AddParticipantsToVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public AddParticipantsToVideoHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(AddParticipantsToVideoHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.CaseType)
                .Include(x => x.Participants).ThenInclude(x=> x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x => x.Participants).ThenInclude(x => x.InterpreterLanguage)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.Update(hearing);
            
            var languages = await _context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            await _hearingService.AddParticipantToService(hearing, command.Participants, languages);
            await _hearingService.CreateParticipantLinks(hearing.Participants.ToList(), command.LinkedParticipants);
            hearing.UpdateBookingStatusJudgeRequirement();
            await _context.SaveChangesAsync();
        }
    }
}