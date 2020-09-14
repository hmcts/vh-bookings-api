using System;
using Bookings.Domain;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class AddEndPointToHearingCommand : ICommand
    {
        public AddEndPointToHearingCommand(Guid hearingId, Endpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = endpoint;
        }

        public Guid HearingId { get; }
        public Endpoint Endpoint { get; }
    }

    public class AddEndPointToHearingCommandHandler : ICommandHandler<AddEndPointToHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public AddEndPointToHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddEndPointToHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
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
