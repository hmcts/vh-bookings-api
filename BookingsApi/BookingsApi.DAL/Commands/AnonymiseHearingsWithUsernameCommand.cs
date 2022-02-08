using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class AnonymiseHearingsWithUsernameCommand : ICommand
    {
        public string Username { get; set; }   
    }

    public class AnonymiseHearingsWithUsernameCommandHandler : ICommandHandler<AnonymiseHearingsWithUsernameCommand>
    {
        private readonly BookingsDbContext _context;

        public AnonymiseHearingsWithUsernameCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AnonymiseHearingsWithUsernameCommand command)
        {
            _context.Persons.Single(p => p.Username == command.Username).AnonymisePersonForSchedulerJob();;
           
            await _context.SaveChangesAsync();
        }
    }
}