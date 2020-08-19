using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class RemoveEndPointFromHearingCommand : ICommand
    {
        public RemoveEndPointFromHearingCommand(Guid hearingId, Guid endpointId)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
        }

        public Guid HearingId { get; set; }
        public Guid EndpointId { get; set; }
    }

    public class RemoveEndPointFromHearingCommandHandler : ICommandHandler<RemoveEndPointFromHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public RemoveEndPointFromHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveEndPointFromHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(hearing => hearing.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var endpoint = hearing.Endpoints.FirstOrDefault(e => e.Id == command.EndpointId);
            if (endpoint == null)
            {
                throw new EndPointNotFoundException(command.EndpointId);
            }

            hearing.RemoveEndpoint(endpoint);
            await _context.SaveChangesAsync();
        }
    }
}
