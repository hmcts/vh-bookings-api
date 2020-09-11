using System;
using Bookings.Domain;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Bookings.DAL.Commands
{
    public class AddEndPointFromHearingCommand : ICommand
    {
        public AddEndPointFromHearingCommand(Guid hearingId, Endpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = endpoint;
        }

        public AddEndPointFromHearingCommand(Guid hearingId, Guid participantId, Endpoint endpoint)
        {
            HearingId = hearingId;
            Endpoint = endpoint;
            ParticipantId = participantId;
        }

        public Guid HearingId { get; set; }
        public Guid? ParticipantId { get; set; }
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
            IQueryable<VideoHearing> query = _context.VideoHearings;
            if(command.ParticipantId.HasValue)
            {
                query = query.Include(h => h.Participants);
            }
            query = query.Include(h => h.Endpoints);

            var hearing = await query.SingleOrDefaultAsync(x => x.Id == command.HearingId);
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            if (command.ParticipantId.HasValue)
            {
                var participant = hearing.Participants.SingleOrDefault(e => e.Id == command.ParticipantId);
                if (participant == null)
                {
                    throw new ParticipantNotFoundException(command.ParticipantId.Value);
                }
                command.Endpoint.AssignDefenceAdvocate(participant);
            }
            hearing.AddEndpoint(command.Endpoint);
            await _context.SaveChangesAsync();
        }
    }
}
