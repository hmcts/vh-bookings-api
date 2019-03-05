using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class RemoveHearingCommand : ICommand
    {
        public RemoveHearingCommand(Guid hearingId)
        {
            HearingId = hearingId;
        }

        public Guid HearingId { get; set; }
    }
    
    public class RemoveHearingCommandHandler : ICommandHandler<RemoveHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public RemoveHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.Remove(hearing);
            await _context.SaveChangesAsync();
        }
    }
}