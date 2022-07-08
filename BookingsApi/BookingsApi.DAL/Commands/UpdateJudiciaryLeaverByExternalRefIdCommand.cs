using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class UpdateJudiciaryLeaverByExternalRefIdCommand : JudiciaryLeaverCommandBase
    {
        public UpdateJudiciaryLeaverByExternalRefIdCommand(string externalRefId, bool leaver) :
            base(externalRefId, leaver)
        {
        }
    }

    public class UpdateJudiciaryLeaverByExternalRefIdHandler : ICommandHandler<UpdateJudiciaryLeaverByExternalRefIdCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateJudiciaryLeaverByExternalRefIdHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateJudiciaryLeaverByExternalRefIdCommand command)
        {
            var person = await _context.JudiciaryPersons.SingleOrDefaultAsync(x => x.ExternalRefId == command.ExternalRefId);

            if (person == null)
            {
                throw new JudiciaryLeaverNotFoundException(command.ExternalRefId);
            }

            person.Update(command.HasLeft);

            await _context.SaveChangesAsync();
        }
    }
}