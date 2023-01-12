using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryPersonByExternalRefIdCommand : UpdateJudiciaryPersonCommandBase
    {
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
            var person =
                await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == command.ExternalRefId);

            if (person == null)
            {
                throw new JudiciaryPersonNotFoundException(command.ExternalRefId);
            }

            person.Update(UpdateJudiciaryPersonDtoMapper.Map(command));

            await _context.SaveChangesAsync();
        }
    }
}