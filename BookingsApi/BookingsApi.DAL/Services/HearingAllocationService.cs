using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Services
{
    public interface IHearingAllocationService
    {
        Task<JusticeUser> AllocateCso(Guid hearingId);
    }
    
    public class HearingAllocationService : IHearingAllocationService
    {
        private readonly BookingsDbContext _context;

        public HearingAllocationService(BookingsDbContext context)
        {
            _context = context;
        }
        
        public async Task<JusticeUser> AllocateCso(Guid hearingId)
        {
            var hearing = await _context.VideoHearings.SingleOrDefaultAsync(x => x.Id == hearingId);
            if (hearing == null)
            {
                throw new ArgumentException($"Hearing {hearingId} not found");   
            }

            // CSOs with work hours that fall within the hearing scheduled time
            var availableCsos = GetAvailableCsos(hearing.ScheduledDateTime);

            if (!availableCsos.Any())
            {
                throw new InvalidOperationException($"Unable to allocate to hearing {hearingId}, no CSOs available");
            }

            return null;
        }

        private IEnumerable<JusticeUser> GetAvailableCsos(DateTime hearingScheduledDatetime)
        {
            var availableCsos = (from justiceUser in _context.JusticeUsers
                let workHoursFallingOnThisDay = justiceUser.VhoWorkHours.FirstOrDefault(h => MapVhDayOfWeekIdToSystemDayOfWeek(h.DayOfWeekId) == hearingScheduledDatetime.DayOfWeek)
                where workHoursFallingOnThisDay != null
                let hearingTime = hearingScheduledDatetime.TimeOfDay
                let workHourStartTime = workHoursFallingOnThisDay.StartTime
                let workHourEndTime = workHoursFallingOnThisDay.EndTime
                where hearingTime > workHourStartTime && hearingTime < workHourEndTime
                select justiceUser).ToList();

            return availableCsos;
        }
        
        private System.DayOfWeek MapVhDayOfWeekIdToSystemDayOfWeek(int vhDayOfWeekId)
        {
            if (vhDayOfWeekId == 7)
            {
                return System.DayOfWeek.Sunday;
            }

            return (System.DayOfWeek)vhDayOfWeekId;
        }
    }
}
