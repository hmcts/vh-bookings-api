using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;

namespace BookingsApi.DAL.Commands
{
    public class UpdateNonWorkingHoursCommand : ICommand
    {
        public Guid JusticeUserId { get; set; }
        public IList<NonWorkHoursDto> Hours { get; set; }

        public UpdateNonWorkingHoursCommand(Guid justiceUserId, IList<NonWorkHoursDto> hours)
        {
            JusticeUserId = justiceUserId;
            Hours = hours;
        }
    }

    public class UpdateNonWorkingHoursCommandHandler : ICommandHandler<UpdateNonWorkingHoursCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingAllocationService _hearingAllocationService;

        public UpdateNonWorkingHoursCommandHandler(BookingsDbContext context, IHearingAllocationService hearingAllocationService)
        {
            _context = context;
            _hearingAllocationService = hearingAllocationService;
        }
        
        public async Task Handle(UpdateNonWorkingHoursCommand command)
        {
            var justiceUser = await _context.JusticeUsers.Include(x=> x.VhoNonAvailability).FirstOrDefaultAsync(x => x.Id == command.JusticeUserId);
            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(command.JusticeUserId);
                
            }
            foreach (var hour in command.Hours)
            {
                var nonWorkingHour = justiceUser.VhoNonAvailability.SingleOrDefault(a => a.Id == hour.Id);
                if (nonWorkingHour == null)
                { 
                    justiceUser.AddOrUpdateNonAvailability(hour.StartTime, hour.EndTime);   
                }
                else
                {
                    nonWorkingHour.Update(hour.StartTime, hour.EndTime);
                }
            }
            
            await _hearingAllocationService.DeallocateFromUnavailableHearings(command.JusticeUserId);

            await _context.SaveChangesAsync();
        }
    }
}
