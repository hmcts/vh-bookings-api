using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
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
            _context.Persons.Single(p => p.Username == command.Username).AnonymisePersonForSchedulerJob();
           
            await _context.SaveChangesAsync();
        }
    }
}