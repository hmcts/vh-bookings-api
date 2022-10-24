using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands.Core;

namespace BookingsApi.DAL.Commands
{
    public class UpdateNonWorkingHoursCommand : ICommand
    {
        public IList<NonWorkingHours> Hours { get; set; }

        public UpdateNonWorkingHoursCommand(IList<NonWorkingHours> hours)
        {
            Hours = hours;
        }
    }

    public class UpdateNonWorkingHoursCommandHandler : ICommandHandler<UpdateNonWorkingHoursCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateNonWorkingHoursCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateNonWorkingHoursCommand command)
        {
            foreach (var hour in command.Hours)
            {
                var nonWorkingHour = _context.VhoNonAvailabilities.SingleOrDefault(a => a.Id == hour.Id);

                nonWorkingHour.StartTime = hour.StartTime;
                nonWorkingHour.EndTime = hour.EndTime;

                _context.Update(nonWorkingHour);
            }

            await _context.SaveChangesAsync();
        }
    }
}
