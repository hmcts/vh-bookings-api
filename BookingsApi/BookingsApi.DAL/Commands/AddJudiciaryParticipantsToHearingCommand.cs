using BookingsApi.Domain.Validations;

namespace BookingsApi.DAL.Commands
{
    public class NewJudiciaryParticipant
    {
        public string DisplayName { get; set; }
        public string PersonalCode { get; set; }
        public JudiciaryParticipantHearingRoleCode HearingRoleCode { get; set; }
    }
    
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

    public class AddJudiciaryParticipantToHearingCommandHandler : ICommandHandler<AddJudiciaryParticipantsToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryParticipantToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddJudiciaryParticipantsToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            foreach (var participant in command.Participants)
            {
                var judiciaryPerson = await _context.JudiciaryPersons
                    .SingleOrDefaultAsync(x => x.PersonalCode == participant.PersonalCode);

                if (judiciaryPerson == null)
                {
                    throw new JudiciaryPersonNotFoundException(participant.PersonalCode);
                }

                switch (participant.HearingRoleCode)
                {
                    case JudiciaryParticipantHearingRoleCode.Judge:
                        hearing.AddJudiciaryJudge(judiciaryPerson, participant.DisplayName);
                        break;
                    case JudiciaryParticipantHearingRoleCode.PanelMember:
                        hearing.AddJudiciaryPanelMember(judiciaryPerson, participant.DisplayName);
                        break;
                    default:
                        throw new DomainRuleException(participant.HearingRoleCode.ToString(),
                            $"Role {participant.HearingRoleCode} not recognised");
                }
            }
  
            await _context.SaveChangesAsync();
        }
    }
}
