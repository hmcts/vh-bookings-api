using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryLeaverByPersonalCodeCommand : JudiciaryLeaverCommandBase
    {
        public UpdateJudiciaryLeaverByPersonalCodeCommand(string personalCode, bool leaver) :
            base(personalCode, leaver)
        {
        }
    }

    public class UpdateJudiciaryLeaverByPersonalCodeHandler : ICommandHandler<UpdateJudiciaryLeaverByPersonalCodeCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateJudiciaryLeaverByPersonalCodeHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateJudiciaryLeaverByPersonalCodeCommand command)
        {
            var person = await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.PersonalCode == command.PersonalCode);

            if (person == null)
            {
                throw new JudiciaryLeaverNotFoundException(command.PersonalCode);
            }

            person.Update(command.HasLeft);

            await _context.SaveChangesAsync();
        }
    }
}