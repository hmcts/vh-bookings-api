using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain;

namespace Bookings.DAL.Commands
{
    public class UpdateHearingCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int ScheduledDuration { get; set; }
        public HearingVenue HearingVenue { get; set; }
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }

        public UpdateHearingCommand(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration,
            HearingVenue hearingVenue, string hearingRoomName, string otherInformation)
        {
            HearingId = hearingId;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            HearingVenue = hearingVenue;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
        }
    }

    public class UpdateHearingCommandHandler : ICommandHandler<UpdateHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateHearingCommand command)
        {
            var hearing = await _context.VideoHearings.FindAsync(command.HearingId);
            hearing.UpdateHearingDetails(command.HearingVenue, command.ScheduledDateTime, command.ScheduledDuration, command.HearingRoomName, command.OtherInformation);
            await _context.SaveChangesAsync();
        }
    }
}