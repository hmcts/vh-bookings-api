using BookingsApi.Domain.Validations;

namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryParticipantToHearingCommand : ICommand
    {
        public AddJudiciaryParticipantToHearingCommand(string displayName, string personalCode, 
            JudiciaryParticipantHearingRoleCode judiciaryParticipantHearingRoleCode, Guid hearingId)
        {
            DisplayName = displayName;
            PersonalCode = personalCode;
            JudiciaryParticipantHearingRoleCode = judiciaryParticipantHearingRoleCode;
            HearingId = hearingId;
        }
        
        public string DisplayName { get; }
        public string PersonalCode { get; }
        public JudiciaryParticipantHearingRoleCode JudiciaryParticipantHearingRoleCode { get; }
        public Guid HearingId { get; }
    }

    public class AddJudiciaryParticipantToHearingCommandHandler : ICommandHandler<AddJudiciaryParticipantToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryParticipantToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddJudiciaryParticipantToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.JudiciaryParticipants).ThenInclude(x => x.JudiciaryPerson)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }
            
            var judiciaryPerson = await _context.JudiciaryPersons
                .SingleOrDefaultAsync(x => x.PersonalCode == command.PersonalCode);

            if (judiciaryPerson == null)
            {
                throw new JudiciaryPersonNotFoundException(command.PersonalCode);
            }

            switch (command.JudiciaryParticipantHearingRoleCode)
            {
                case JudiciaryParticipantHearingRoleCode.Judge:
                    hearing.AddJudiciaryJudge(judiciaryPerson, command.DisplayName);
                    break;
                case JudiciaryParticipantHearingRoleCode.PanelMember:
                    hearing.AddJudiciaryPanelMember(judiciaryPerson, command.DisplayName);
                    break;
                default:
                    throw new DomainRuleException(command.JudiciaryParticipantHearingRoleCode.ToString(),
                        $"Role {command.JudiciaryParticipantHearingRoleCode} not recognised");
            }

            await _context.SaveChangesAsync();
        }
    }
}
