using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
{
    public class DeleteNonWorkingHoursCommand : ICommand
    {
        public long Id { get; }
        public List<string> FailedUploadUsernames { get; set; } = new List<string>();

        public DeleteNonWorkingHoursCommand(long id)
        {
            Id = id;
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
            var hours = await _context.VhoNonAvailabilities
                .FirstOrDefaultAsync(x => x.Id == command.Id);
            
            if (hours == null)
            {
                throw new NonWorkingHoursNotFoundException(command.Id);
            }

            hours.Delete();
            
            _context.Update(hours);

            await _context.SaveChangesAsync();
        }
    }
}