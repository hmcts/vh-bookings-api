using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class AnonymisePersonCommand : ICommand
    {
        public string Username { get; }

        public AnonymisePersonCommand(string username)
        {
            Username = username;
        }
    }

    public class AnonymisePersonCommandHandler : ICommandHandler<AnonymisePersonCommand>
    {
        private readonly BookingsDbContext _context;

        public AnonymisePersonCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymisePersonCommand command)
        {
            var username = command.Username.ToLower().Trim();
            var person = await _context.Persons.SingleOrDefaultAsync(x => x.Username.ToLower().Trim() == username);

            if (person == null)
            {
                throw new PersonNotFoundException(username);
            }
            
            person.AnonymisePerson();
            await _context.SaveChangesAsync();
        }

        
    }


}