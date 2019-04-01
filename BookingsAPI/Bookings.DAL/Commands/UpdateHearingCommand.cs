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
        public string UpdatedBy { get; set; }

        public UpdateHearingCommand(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration,
            HearingVenue hearingVenue, string hearingRoomName, string otherInformation, string updatedBy)
        {
            HearingId = hearingId;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            HearingVenue = hearingVenue;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdatedBy = updatedBy;
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
            hearing.UpdateHearingDetails(command.HearingVenue, command.ScheduledDateTime, command.ScheduledDuration, command.HearingRoomName, command.OtherInformation, command.UpdatedBy);
            await _context.SaveChangesAsync();
        }
    }
}