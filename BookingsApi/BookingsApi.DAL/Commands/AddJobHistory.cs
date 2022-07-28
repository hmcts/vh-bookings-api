using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Domain;

namespace BookingsApi.DAL.Commands
{
    public class AddJobHistoryCommand : ICommand
    {
        public string JobName { get; set; }   
        public bool IsSuccessful { get; set; }   
    }

    public class AddJobHistoryCommandHandler : ICommandHandler<AddJobHistoryCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJobHistoryCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AddJobHistoryCommand command)
        {
            await _context.JobHistory.AddAsync( new UpdateJobHistory(command.JobName, command.IsSuccessful));
            await _context.SaveChangesAsync();
        }
    }
}