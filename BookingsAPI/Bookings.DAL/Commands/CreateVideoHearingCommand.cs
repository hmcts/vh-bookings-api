using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain;
using Bookings.Domain.RefData;

namespace Bookings.DAL.Commands
{
    public class CreateVideoHearingCommand : ICommand
    {
        public CreateVideoHearingCommand(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue venue, List<NewParticipant> participants, List<Case> cases)
        {
            CaseType = caseType;
            HearingType = hearingType;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            Venue = venue;
            Participants = participants;
            Cases = cases;
        }

        public Guid NewHearingId { get; set; }
        public CaseType CaseType { get; }
        public HearingType HearingType { get; }
        public DateTime ScheduledDateTime { get; }
        public int ScheduledDuration { get; }
        public HearingVenue Venue { get; }
        public List<NewParticipant> Participants { get; }
        public List<Case> Cases { get; }
    }

    public class CreateVideoHearingCommandHandler : ICommandHandler<CreateVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public CreateVideoHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(CreateVideoHearingCommand command)
        {
            var videoHearing = new VideoHearing(command.CaseType, command.HearingType, command.ScheduledDateTime,
                command.ScheduledDuration, command.Venue);
            await _hearingService.AddParticipantToService(videoHearing, command.Participants);
            videoHearing.AddCases(command.Cases);
            _context.VideoHearings.Add(videoHearing);
            await _context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
        }
    }
}