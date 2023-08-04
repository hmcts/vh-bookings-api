namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryPersonByPersonalCodeCommand : JudiciaryPersonCommandBase
    {
        public AddJudiciaryPersonByPersonalCodeCommand(string externalRefId, string personalCode, string title,
            string knownAs, string surname, string fullname, string postNominals, string email, bool hasLeft, bool leaver, string leftOn) :
            base(externalRefId, personalCode, title, knownAs, surname, fullname, postNominals, email, hasLeft, leaver, leftOn)
        {
        }
    }

    public class AddJudiciaryPersonByPersonalCodeHandler : ICommandHandler<AddJudiciaryPersonByPersonalCodeCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryPersonByPersonalCodeHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(AddJudiciaryPersonByPersonalCodeCommand command)
        {
            var judiciaryPerson = new JudiciaryPerson(command.ExternalRefId, command.PersonalCode, command.Title, command.KnownAs, command.Surname, command.Fullname, command.PostNominals, command.Email, command.HasLeft, command.Leaver, command.LeftOn);

            await _context.JudiciaryPersons.AddAsync(judiciaryPerson);

            await _context.SaveChangesAsync();
        }
    }
}