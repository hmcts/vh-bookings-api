using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;

namespace BookingsApi.DAL.Commands.V1
{
    public class UpdateNonWorkingHoursCommand : ICommand
    {
        public Guid JusticeUserId { get; set; }
        public IList<NonWorkingHours> Hours { get; set; }

        public UpdateNonWorkingHoursCommand(Guid justiceUserId, IList<NonWorkingHours> hours)
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
            foreach (var hour in command.Hours)
            {
                var nonWorkingHour = _context.VhoNonAvailabilities.SingleOrDefault(a => a.Id == hour.Id);

                var isNewNonWorkingHourEntry = nonWorkingHour == null;

                if (isNewNonWorkingHourEntry)
                {
                    nonWorkingHour = new VhoNonAvailability();
                    nonWorkingHour.JusticeUserId = command.JusticeUserId;
                }
                
                nonWorkingHour.StartTime = hour.StartTime;
                nonWorkingHour.EndTime = hour.EndTime;

                if (isNewNonWorkingHourEntry)
                    _context.Add(nonWorkingHour);
                else
                    _context.Update(nonWorkingHour);
            }
            
            await _hearingAllocationService.DeallocateFromUnavailableHearings(command.JusticeUserId);

            await _context.SaveChangesAsync();
        }
    }
}
