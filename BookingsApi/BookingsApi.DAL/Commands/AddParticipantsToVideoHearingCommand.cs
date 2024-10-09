using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.SpecialMeasure;

namespace BookingsApi.DAL.Commands
{
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

            _context.Update(hearing);
            
            var languages = await _context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            await _hearingService.AddParticipantToService(hearing, command.Participants, languages);
            await _hearingService.CreateParticipantLinks(hearing.Participants.ToList(), command.LinkedParticipants);
            hearing.UpdateBookingStatusJudgeRequirement();
            
            foreach (var participantForScreening in command.Participants.Where(x=> x.Screening != null))
            {
                var participant = hearing.GetParticipants().Single(x=> x.Person.ContactEmail == participantForScreening.Person.ContactEmail);
                _hearingService.UpdateParticipantScreeningRequirement(hearing, participant, participantForScreening.Screening);
            }
            
            await _context.SaveChangesAsync();
        }
    }
}