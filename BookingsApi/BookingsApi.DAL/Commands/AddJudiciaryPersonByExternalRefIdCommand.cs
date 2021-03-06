using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class AddJudiciaryPersonByExternalRefIdCommand : JudiciaryPersonCommandBase
    {
        public AddJudiciaryPersonByExternalRefIdCommand(Guid externalRefId, string personalCode, string title,
            string knownAs, string surname, string fullname, string postNominals, string email, bool hasLeft) :
            base(externalRefId, personalCode, title, knownAs, surname, fullname, postNominals, email, hasLeft)
        {
        }
    }

    public class AddJudiciaryPersonByExternalRefIdHandler : ICommandHandler<AddJudiciaryPersonByExternalRefIdCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJudiciaryPersonByExternalRefIdHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(AddJudiciaryPersonByExternalRefIdCommand command)
        {
            var judiciaryPerson = new JudiciaryPerson(command.ExternalRefId, command.PersonalCode, command.Title, command.KnownAs, command.Surname, command.Fullname, command.PostNominals, command.Email, command.HasLeft);

            await _context.JudiciaryPersons.AddAsync(judiciaryPerson);

            await _context.SaveChangesAsync();
        }
    }
}