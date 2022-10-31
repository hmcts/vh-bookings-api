using System;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Exceptions;

namespace BookingsApi.DAL.Commands
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

            hours.Deleted = true;
            
            _context.Update(hours);

            await _context.SaveChangesAsync();
        }
    }
}