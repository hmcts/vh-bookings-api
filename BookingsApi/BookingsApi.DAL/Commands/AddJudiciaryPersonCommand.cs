namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryPersonCommand : JudiciaryPersonCommandBase
    {
        public AddJudiciaryPersonCommand(string externalRefId, string personalCode, string title, 
            string knownAs, string surname, string fullname, string postNominals, string email, string workPhone, bool hasLeft, bool leaver, string leftOn) : 
            base(externalRefId, personalCode, title, knownAs, surname, fullname, postNominals, email, workPhone, hasLeft, leaver, leftOn)
        {
        }
    }

    public class AddJudiciaryPersonCommandHandler : ICommandHandler<AddJudiciaryPersonCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryPersonCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(AddJudiciaryPersonCommand command)
        {
            await _context.JudiciaryPersons.AddAsync(new JudiciaryPerson(command.ExternalRefId, command.PersonalCode,
                command.Title, command.KnownAs, command.Surname, command.Fullname, command.PostNominals, command.Email,
                command.WorkPhone, command.HasLeft, command.Leaver, command.LeftOn));

            await _context.SaveChangesAsync();
        }
    }
}