namespace BookingsApi.DAL.Commands
{
    public class RestoreJusticeUserCommand : ICommand
    {
        public Guid Id { get; }

        public RestoreJusticeUserCommand(Guid id)
        {
            Id = id;
        }
    }
    
    public class RestoreJusticeUserCommandHandler : ICommandHandler<RestoreJusticeUserCommand>
    {
        private readonly BookingsDbContext _context;

        public RestoreJusticeUserCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RestoreJusticeUserCommand command)
        {
            var justiceUser = await _context.JusticeUsers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == command.Id);

            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(command.Id);
            }
            
            justiceUser.Restore();
      
            await _context.SaveChangesAsync();
        }
    }
}
