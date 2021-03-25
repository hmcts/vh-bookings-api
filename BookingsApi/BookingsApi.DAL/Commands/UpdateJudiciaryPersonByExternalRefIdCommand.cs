using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryPersonByExternalRefIdCommand : JudiciaryPersonCommandBase
    {
        public UpdateJudiciaryPersonByExternalRefIdCommand(Guid externalRefId, string personalCode, string title, 
            string knownAs, string surname, string fullname, string postNominals, string email) :
            base(externalRefId, personalCode, title, knownAs, surname, fullname, postNominals, email)
        {
        }
    }

    public class UpdateJudiciaryPersonByExternalRefIdHandler : ICommandHandler<UpdateJudiciaryPersonByExternalRefIdCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateJudiciaryPersonByExternalRefIdHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateJudiciaryPersonByExternalRefIdCommand command)
        {
            var person = await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == command.ExternalRefId);

            if (person == null)
            {
                throw new JudiciaryPersonNotFoundException(command.ExternalRefId);
            }

            person.Update(command.PersonalCode, command.Title, command.KnownAs, command.Surname, command.Fullname, command.PostNominals, command.Email);

            await _context.SaveChangesAsync();
        }
    }
}