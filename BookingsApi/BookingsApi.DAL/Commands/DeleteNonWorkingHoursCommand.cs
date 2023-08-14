namespace BookingsApi.DAL.Commands
{
    public class DeleteNonWorkingHoursCommand : ICommand
    {
        public Guid JusticeUserId { get; }
        public long NonAvailableHourId { get; }

        public DeleteNonWorkingHoursCommand(Guid justiceUserId, long nonAvailableHourId)
        {
            JusticeUserId = justiceUserId;
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
            var justiceUser = await _context.JusticeUsers.FirstOrDefaultAsync(x => x.Id == command.JusticeUserId);
            
            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(command.JusticeUserId);
                
            }

            var nonAvailability = justiceUser.VhoNonAvailability.FirstOrDefault(x => x.Id == command.NonAvailableHourId);
            if (nonAvailability == null)
            {
                throw new NonWorkingHoursNotFoundException(command.NonAvailableHourId);
            }
            
            nonAvailability.Delete();
            await _context.SaveChangesAsync();
        }
    }
}