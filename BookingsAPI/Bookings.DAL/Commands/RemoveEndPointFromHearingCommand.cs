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

        public Guid HearingId { get; }
        public Guid EndpointId { get; }
        public string RemoveSip { get; protected internal set; }
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
                .Include(h => h.Endpoints).ThenInclude(x=> x.DefenceAdvocate)
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
            command.RemoveSip = endpoint.Sip;
            await _context.SaveChangesAsync();
        }
    }
}
