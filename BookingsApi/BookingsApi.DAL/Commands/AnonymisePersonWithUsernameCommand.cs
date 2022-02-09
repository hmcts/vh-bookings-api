using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class AnonymisePersonWithUsernameCommand : ICommand
    {
        public string Username { get; set; }   
    }

    public class AnonymisePersonWithUsernameCommandHandler : ICommandHandler<AnonymisePersonWithUsernameCommand>
    {
        private readonly BookingsDbContext _context;

        public AnonymisePersonWithUsernameCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AnonymisePersonWithUsernameCommand command)
        {
            var person = await _context.Persons.SingleOrDefaultAsync(p => p.Username == command.Username);

            if (person == null)
            {
                throw new PersonNotFoundException(command.Username);
            }
            
            person.AnonymisePersonForSchedulerJob();
           
            await _context.SaveChangesAsync();
        }
    }
}