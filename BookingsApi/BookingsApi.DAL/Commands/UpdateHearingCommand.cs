using BookingsApi.DAL.Services;

namespace BookingsApi.DAL.Commands
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
        public List<Case> Cases { get; set; }
        public bool AudioRecordingRequired { get; set; }

        public UpdateHearingCommand(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration,
            HearingVenue hearingVenue, string hearingRoomName, string otherInformation, string updatedBy,
            List<Case> cases, bool audioRecordingRequired)
        {
            HearingId = hearingId;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            HearingVenue = hearingVenue;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdatedBy = updatedBy;
            Cases = cases;
            AudioRecordingRequired = audioRecordingRequired;
        }
    }

    public class UpdateHearingCommandHandler : ICommandHandler<UpdateHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingAllocationService _hearingAllocationService;

        public UpdateHearingCommandHandler(BookingsDbContext context, IHearingAllocationService hearingAllocationService)
        {
            _context = context;
            _hearingAllocationService = hearingAllocationService;
        }

        public async Task Handle(UpdateHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x=>x.HearingCases).ThenInclude(x=> x.Case)
                .Include(x => x.Allocations).ThenInclude(x => x.JusticeUser).ThenInclude(x => x.VhoWorkHours)
                .Include(x => x.Allocations).ThenInclude(x => x.JusticeUser).ThenInclude(x => x.VhoNonAvailability)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var oldScheduledDateTime = hearing.ScheduledDateTime;

            hearing.UpdateHearingDetails(command.HearingVenue, command.ScheduledDateTime,
                command.ScheduledDuration, command.HearingRoomName, command.OtherInformation,
                command.UpdatedBy, command.Cases, command.AudioRecordingRequired);

            if (command.ScheduledDateTime != oldScheduledDateTime)
            {
                _hearingAllocationService.CheckAndDeallocateHearing(hearing);
            }

            await _context.SaveChangesAsync();
        }
    }
}