namespace BookingsApi.DAL.Commands;

public class DeleteTestPersonCommand : ICommand
{
    public string Username { get; }
    public DeleteTestPersonCommand(string username) 
    {
        Username = username;
    }
}

public class DeleteTestPersonCommandHandler : ICommandHandler<DeleteTestPersonCommand>
{
    private readonly BookingsDbContext _context;

    public DeleteTestPersonCommandHandler(BookingsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTestPersonCommand command)
    {
        var person = await _context.Persons.SingleOrDefaultAsync(p => p.Username == command.Username);
        
        if (person == null)
            throw new PersonNotFoundException(command.Username);
        
        var participants = await _context.Participants
            .Include(p => p.Hearing).ThenInclude(h => h.CaseType)
            .Where(p => p.PersonId == person.Id)
            .ToListAsync();
        
        // If the person is a participant on a case type that is not generic, then it is not a valid Test User
        if(participants.Exists(p => p.Hearing?.CaseType?.Name != "Generic"))
            throw new ArgumentException(command.Username);
        
        _context.Participants.RemoveRange(participants);
        _context.Persons.Remove(person);
        
        await _context.SaveChangesAsync();
    }
}