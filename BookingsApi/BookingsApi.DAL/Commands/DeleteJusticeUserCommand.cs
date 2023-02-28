using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class DeleteJusticeUserCommand : ICommand
    {
        public Guid Id { get; }

        public DeleteJusticeUserCommand(Guid id)
        {
            Id = id;
        }
    }
    
    public class DeleteJusticeUserCommandHandler : ICommandHandler<DeleteJusticeUserCommand>
    {
        private readonly BookingsDbContext _context;

        public DeleteJusticeUserCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteJusticeUserCommand command)
        {
            var justiceUser = await _context.JusticeUsers
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations).ThenInclude(a => a.Hearing)
                .FirstOrDefaultAsync(u => u.Id == command.Id);

            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(command.Id);
            }
            
            justiceUser.Delete();
      
            await _context.SaveChangesAsync();
        }
    }
}
