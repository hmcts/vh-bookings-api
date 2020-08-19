using System;
using System.Linq;
using Bookings.Domain;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class AddEndPointFromHearingCommand : ICommand
    {
        public AddEndPointFromHearingCommand(Guid hearingId, Endpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = endpoint;
        }

        public Guid HearingId { get; set; }
        public Endpoint Endpoint { get; set; }
    }

    public class AddEndPointFromHearingCommandHandler : ICommandHandler<AddEndPointFromHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddEndPointFromHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddEndPointFromHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(hearing => hearing.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            hearing.AddEndpoint(command.Endpoint);
            await _context.SaveChangesAsync();
        }
    }
}
