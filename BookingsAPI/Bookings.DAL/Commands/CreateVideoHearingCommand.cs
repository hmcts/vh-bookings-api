using System;
using System.Threading.Tasks;
using Bookings.Domain;
using Bookings.Domain.RefData;

namespace Bookings.DAL.Commands
{
    public class CreateVideoHearingCommand : ICommand
    {
        public CreateVideoHearingCommand(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue venue)
        {
            CaseType = caseType;
            HearingType = hearingType;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            Venue = venue;
        }

        public Guid NewHearingId { get; set; }
        public CaseType CaseType { get; }
        public HearingType HearingType { get; }
        public DateTime ScheduledDateTime { get; }
        public int ScheduledDuration { get; }
        public HearingVenue Venue { get; }
    }

    public class CreateVideoHearingCommandHandler : ICommandHandler<CreateVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public CreateVideoHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateVideoHearingCommand command)
        {
            var videoHearing = new VideoHearing(command.CaseType, command.HearingType, command.ScheduledDateTime,
                command.ScheduledDuration, command.Venue);
            _context.VideoHearings.Add(videoHearing);
            
            await _context.SaveChangesAsync();

            command.NewHearingId = videoHearing.Id;
        }
    }
}