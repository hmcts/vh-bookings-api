namespace BookingsApi.DAL.Commands
{
    public class DeleteNonWorkingHoursCommand : ICommand
    {
        public string Username { get; }
        public long NonAvailableHourId { get; }

        public DeleteNonWorkingHoursCommand(string username, long nonAvailableHourId)
        {
            Username = username;
            NonAvailableHourId = nonAvailableHourId;
        }
    }

    public class DeleteNonWorkingHoursCommandHandler : ICommandHandler<DeleteNonWorkingHoursCommand>
    {
        private readonly BookingsDbContext _context;

        public DeleteNonWorkingHoursCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteNonWorkingHoursCommand command)
        {
            var justiceUser = await _context.JusticeUsers.Include(x => x.VhoNonAvailability)
                .FirstOrDefaultAsync(x => x.Username == command.Username);
            
            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(command.Username);
                
            }

            var nonAvailability = justiceUser.VhoNonAvailability.FirstOrDefault(x => x.Id == command.NonAvailableHourId);
            if (nonAvailability == null)
            {
                throw new NonWorkingHoursNotFoundException(command.NonAvailableHourId);
            }
            
            justiceUser.RemoveNonAvailability(nonAvailability);
            await _context.SaveChangesAsync();
        }
    }
}