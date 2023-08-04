namespace BookingsApi.DAL.Commands
{
    public class UpdatePersonUsernameCommand : ICommand
    {
        public Guid PersonId { get; }
        public string Username { get; }
        
        public UpdatePersonUsernameCommand(Guid personId, string username)
        {
            PersonId = personId;
            Username = username ?? throw new ArgumentNullException(nameof(username));
        }
    }

    public class UpdatePersonUsernameCommandHandler : ICommandHandler<UpdatePersonUsernameCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdatePersonUsernameCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdatePersonUsernameCommand command)
        {
            var person = await _context.Persons.FindAsync(command.PersonId);

            if (person == null)
            {
                throw new PersonNotFoundException(command.PersonId);
            }

            person.UpdatePerson(command.Username);

            await _context.SaveChangesAsync();
        }
    }
}