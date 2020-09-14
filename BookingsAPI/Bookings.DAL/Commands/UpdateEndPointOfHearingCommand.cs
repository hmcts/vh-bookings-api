using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bookings.Domain.Participants;

namespace Bookings.DAL.Commands
{
    public class UpdateEndPointOfHearingCommand : ICommand
    {
        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName, Participant defenceAdvocate)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
            DefenceAdvocate = defenceAdvocate;
        }

        public Guid HearingId { get; }
        public Guid EndpointId { get;  }
        public string DisplayName { get;  }
        public Participant DefenceAdvocate { get; }
}

    public class UpdateEndPointOfHearingCommandHandler : ICommandHandler<UpdateEndPointOfHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateEndPointOfHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateEndPointOfHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.Participants).ThenInclude(x => x.Person)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var endpoint = hearing.Endpoints.SingleOrDefault(e => e.Id == command.EndpointId);
            if (endpoint == null)
            {
                throw new EndPointNotFoundException(command.EndpointId);
            }

            if (!string.IsNullOrWhiteSpace(endpoint.DisplayName)) endpoint.UpdateDisplayName(command.DisplayName);
            if (command.DefenceAdvocate != null)
            {
                var defenceAdvocate = hearing.GetParticipants().Single(x => x.Id == command.DefenceAdvocate.Id);
                endpoint.AssignDefenceAdvocate(defenceAdvocate);
            }

            await _context.SaveChangesAsync();
        }
    }
}
