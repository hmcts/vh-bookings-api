namespace BookingsApi.DAL.Commands
{
    public class UpdateHearingStatusCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public BookingStatus Status { get; set; }
        public string UpdatedBy { get; set; }
        public string CancelReason { get; set; }
        public UpdateHearingStatusCommand(Guid hearingId, BookingStatus status, string updatedBy, string cancelReason)
        {
            HearingId = hearingId;
            Status = status;
            UpdatedBy = updatedBy;
            CancelReason = cancelReason;
        }
    }

    public class UpdateHearingStatusCommandHandler : ICommandHandler<UpdateHearingStatusCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateHearingStatusCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateHearingStatusCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Allocations)
                .Include(x => x.Participants)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            hearing.UpdateStatus(command.Status, command.UpdatedBy, command.CancelReason);
            await _context.SaveChangesAsync();
        }
    }
}