using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
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

            var jobHistoryEntry = await _context.JobHistory.FirstOrDefaultAsync() as UpdateJobHistory;

            if (jobHistoryEntry == null)
            {
                await _context.JobHistory.AddAsync(new UpdateJobHistory());
            }
            else
            {
                jobHistoryEntry.UpdateLastRunDate();    
            }
           
            await _context.SaveChangesAsync();
        }
    }
}