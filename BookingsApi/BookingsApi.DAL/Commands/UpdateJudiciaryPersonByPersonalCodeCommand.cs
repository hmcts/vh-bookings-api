using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryPersonByPersonalCodeCommand : UpdateJudiciaryPersonCommandBase
    {
    }

    public class UpdateJudiciaryPersonByPersonalCodeHandler : ICommandHandler<UpdateJudiciaryPersonByPersonalCodeCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateJudiciaryPersonByPersonalCodeHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateJudiciaryPersonByPersonalCodeCommand command)
        {
            var person =
                await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.PersonalCode == command.PersonalCode);

            if (person == null)
            {
                throw new JudiciaryPersonNotFoundException(command.PersonalCode);
            }

            person.Update(UpdateJudiciaryPersonDtoMapper.Map(command));

            await _context.SaveChangesAsync();
        }
    }
}