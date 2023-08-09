using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;

namespace BookingsApi.DAL.Commands
{
    public class UploadWorkHoursCommand : ICommand
    {
        public List<UploadWorkHoursDto> UploadWorkHoursRequests { get; }
        public List<string> FailedUploadUsernames { get; } = new();

        public UploadWorkHoursCommand(List<UploadWorkHoursDto> uploadWorkHoursRequests)
        {
            UploadWorkHoursRequests = uploadWorkHoursRequests;
        }
    }

    public class UploadWorkHoursCommandHandler : ICommandHandler<UploadWorkHoursCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingAllocationService _hearingAllocationService;

        public UploadWorkHoursCommandHandler(BookingsDbContext context, IHearingAllocationService hearingAllocationService)
        {
            _context = context;
            _hearingAllocationService = hearingAllocationService;
        }

        public async Task Handle(UploadWorkHoursCommand command)
        {
            foreach (var uploadWorkHoursRequest in command.UploadWorkHoursRequests)
            {
                var user = await _context.JusticeUsers.Include(x=> x.VhoWorkHours)
                    .SingleOrDefaultAsync(x => x.Username == uploadWorkHoursRequest.Username);

                if (user == null)
                {
                    command.FailedUploadUsernames.Add(uploadWorkHoursRequest.Username);
                    continue;
                }

                foreach (var workHours in uploadWorkHoursRequest.WorkingHours)
                {
                    var vhoWorkHour = user.VhoWorkHours
                        .SingleOrDefault(x => x.DayOfWeekId == workHours.DayOfWeekId);

                    if (vhoWorkHour == null)
                    {
                        vhoWorkHour = new VhoWorkHours()
                        {
                            DayOfWeekId = workHours.DayOfWeekId,
                            JusticeUserId = user.Id,
                            StartTime = workHours.StartTime,
                            EndTime = workHours.EndTime
                        };
                        user.VhoWorkHours.Add(vhoWorkHour);
                    }
                    else
                    {
                        vhoWorkHour.StartTime = workHours.StartTime;
                        vhoWorkHour.EndTime = workHours.EndTime;
                    }
                }

                await _hearingAllocationService.DeallocateFromUnavailableHearings(user.Id);
            }
            await _context.SaveChangesAsync();
        }
    }
}