using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;

namespace BookingsApi.DAL.Commands
{
    public class UpdatePersonCommand : ICommand
    {
        public Guid PersonId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Username { get; }
        
        public UpdatePersonCommand(Guid personId, string firstName, string lastName, string username)
        {
            PersonId = personId;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Username = username ?? throw new ArgumentNullException(nameof(username));
        }
    }

    public class UpdatePersonCommandHandler : ICommandHandler<UpdatePersonCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdatePersonCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdatePersonCommand command)
        {
            var person = await _context.Persons.FindAsync(command.PersonId);

            if (person == null)
            {
                throw new PersonNotFoundException(command.PersonId);
            }

            person.UpdatePerson(command.FirstName, command.LastName, person.ContactEmail, command.Username);

            await _context.SaveChangesAsync();
        }
    }
}