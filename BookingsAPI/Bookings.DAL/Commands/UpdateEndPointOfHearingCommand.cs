using System;
using Bookings.Domain;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bookings.DAL.Commands
{
    public class UpdateEndPointOfHearingCommand : ICommand
    {
        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
        }

        public UpdateEndPointOfHearingCommand(Guid hearingId, Guid endpointId, string displayName, Guid participantId)
        {
            HearingId = hearingId;
            EndpointId = endpointId;
            DisplayName = displayName;
            ParticipantId = participantId;
        }

        public Guid HearingId { get; set; }
        public Guid EndpointId { get; set; }
        public string DisplayName { get; set; }
        public Guid? ParticipantId { get; set; }
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
            IQueryable<VideoHearing> query = _context.VideoHearings;
            if (command.ParticipantId.HasValue)
            {
                query = query.Include(h => h.Participants);
            }
            query = query.Include(h => h.Endpoints);
            var hearing = await query.SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var endpoint = hearing.Endpoints.SingleOrDefault(e => e.Id == command.EndpointId);
            if (endpoint == null)
            {
                throw new EndPointNotFoundException(command.EndpointId);
            }

            endpoint.UpdateDisplayName(command.DisplayName);
            if (command.ParticipantId.HasValue)
            {
                var participant = hearing.Participants.SingleOrDefault(e => e.Id == command.ParticipantId);
                if (participant == null)
                {
                    throw new ParticipantNotFoundException(command.ParticipantId.Value);
                }
                endpoint.AssignDefenceAdvocate(participant);
            }
            else
            {
                endpoint.AssignDefenceAdvocate(null);
            }
            await _context.SaveChangesAsync();
        }
    }
}
