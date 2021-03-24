using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Domain;

namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryPersonCommand : JudiciaryPersonCommandBase
    {
        public AddJudiciaryPersonCommand(Guid externalRefId, string personalCode, string title, 
            string knownAs, string surname, string fullname, string postNominals, string email) : 
            base(externalRefId, personalCode, title, knownAs, surname, fullname, postNominals, email)
        {
        }
    }

    public class AddJudiciaryPersonCommandHandler : ICommandHandler<AddJudiciaryPersonCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryPersonCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(AddJudiciaryPersonCommand command)
        {
            await _context.JudiciaryPersons.AddAsync(new JudiciaryPerson(command.ExternalRefId, command.PersonalCode,
                command.Title, command.KnownAs, command.Surname, command.Fullname, command.PostNominals, command.Email));

            await _context.SaveChangesAsync();
        }
    }
}