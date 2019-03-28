using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain;

namespace Bookings.DAL.Commands
{
    public class UpdateParticipantCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public Guid ParticpantId { get; set; }

        public UpdateParticipantCommand(Guid hearingId, Guid participantId)
        {
            HearingId = hearingId;
            ParticpantId = participantId;
        }
    }

    public class UpdateParticipantCommandHandler : ICommandHandler<UpdateHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateParticipantCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateHearingCommand command)
        {
            var hearing = await _context.VideoHearings.FindAsync(command.HearingId);
            await _context.SaveChangesAsync();
        }
    }
}