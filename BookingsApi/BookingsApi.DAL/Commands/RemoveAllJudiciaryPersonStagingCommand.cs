using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class RemoveAllJudiciaryPersonStagingCommand : ICommand
    {
        
    }

    public class RemoveAllJudiciaryPersonStagingCommandHandler : ICommandHandler<RemoveAllJudiciaryPersonStagingCommand>
    {
        private readonly BookingsDbContext _context;

        public RemoveAllJudiciaryPersonStagingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task Handle(RemoveAllJudiciaryPersonStagingCommand command)
        {
           await  _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE JudiciaryPersonsStaging");
        }
    }
}