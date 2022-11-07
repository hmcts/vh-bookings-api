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

    public interface IRandomNumberGenerator
    {
        int Generate(int max);
    }

    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        public int Generate(int max)
        {
            return new Random().Next(1, max);
        }
    }
    
    public class HearingAllocationService : IHearingAllocationService
    {
        private readonly BookingsDbContext _context;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public HearingAllocationService(BookingsDbContext context, IRandomNumberGenerator randomNumberGenerator)
        {
            _context = context;
            _randomNumberGenerator = randomNumberGenerator;
        }
        
        public async Task<JusticeUser> AllocateCso(Guid hearingId)
        {
            var hearing = await _context.VideoHearings.SingleOrDefaultAsync(x => x.Id == hearingId);
            if (hearing == null)
            {
                throw new ArgumentException($"Hearing {hearingId} not found");   
            }

            //var allocations = _context.Allocations.ToList();
            
            // CSOs with work hours that fall within the hearing scheduled time
            var availableCsos = GetAvailableCsos(hearing.ScheduledDateTime);

            if (!availableCsos.Any())
            {
                throw new InvalidOperationException($"Unable to allocate to hearing {hearingId}, no CSOs available");
            }

            if (availableCsos.Count() == 1)
            {
                return availableCsos.SingleOrDefault();
            }

            if (availableCsos.Count() > 1)
            {
                var csosWithNoAllocations = new List<JusticeUser>();
                
                foreach (var availableCso in availableCsos)
                {
                    // Get allocations for this cso
                    var allocations = _context.Allocations
                        .Where(a => a.JusticeUserId == availableCso.Id)
                        .ToList();
                    if (!allocations.Any())
                    {
                        csosWithNoAllocations.Add(availableCso); 
                    }
                }

                if (csosWithNoAllocations.Count == 1)
                {
                    return csosWithNoAllocations.SingleOrDefault();
                }

                if (csosWithNoAllocations.Count > 1)
                {
                    return AllocateRandomly(csosWithNoAllocations);
                }

                var csosWithOneAllocation = new List<JusticeUser>();
                
                foreach (var availableCso in availableCsos)
                {
                    // Get allocations for this cso
                    var allocations = _context.Allocations
                        .Where(a => a.JusticeUserId == availableCso.Id)
                        .ToList();
                    if (allocations.Count == 1)
                    {
                        csosWithOneAllocation.Add(availableCso); 
                    }
                }

                if (csosWithOneAllocation.Count == 1)
                {
                    return csosWithOneAllocation.SingleOrDefault();
                }

                if (csosWithOneAllocation.Count > 1)
                {
                    return AllocateRandomly(csosWithOneAllocation);
                }
                
                var csosWithTwoAllocations = new List<JusticeUser>();
                
                foreach (var availableCso in availableCsos)
                {
                    // Get allocations for this cso
                    var allocations = _context.Allocations
                        .Where(a => a.JusticeUserId == availableCso.Id)
                        .ToList();
                    if (allocations.Count == 2)
                    {
                        csosWithTwoAllocations.Add(availableCso); 
                    }
                }

                if (csosWithTwoAllocations.Count == 1)
                {
                    return csosWithTwoAllocations.SingleOrDefault();
                }
                
                if (csosWithTwoAllocations.Count > 1)
                {
                    return AllocateRandomly(csosWithTwoAllocations);
                }
            }

            throw new InvalidOperationException($"Unable to allocate to hearing {hearingId}, no CSOs available");
        }

        private IEnumerable<JusticeUser> GetAvailableCsos(DateTime hearingScheduledDatetime)
        {
            var users = _context.JusticeUsers.ToList(); // For testing/debugging only

            var hearingTime = hearingScheduledDatetime.TimeOfDay;
            
            var availableCsos = new List<JusticeUser>();

            foreach (var justiceUser in _context.JusticeUsers)
            {
                var workHoursFallingOnThisDay = justiceUser.VhoWorkHours.FirstOrDefault(h => DayOfWeekIdToSystemDayOfWeek(h.DayOfWeekId) == hearingScheduledDatetime.DayOfWeek);
                if (workHoursFallingOnThisDay == null)
                {
                    continue;
                }

                if (justiceUser.VhoNonAvailability.Any(na => hearingScheduledDatetime > na.StartTime && hearingScheduledDatetime < na.EndTime))
                {
                    continue;
                }
                
                var workHourStartTime = workHoursFallingOnThisDay.StartTime;
                var workHourEndTime = workHoursFallingOnThisDay.EndTime;

                if (hearingTime > workHourStartTime && hearingTime < workHourEndTime)
                {
                    availableCsos.Add(justiceUser);
                }
            }

            return availableCsos;
        }
        
        private System.DayOfWeek DayOfWeekIdToSystemDayOfWeek(int dayOfWeekId)
        {
            if (dayOfWeekId == 7)
            {
                return System.DayOfWeek.Sunday;
            }

            return (System.DayOfWeek)dayOfWeekId;
        }

        private JusticeUser AllocateRandomly(IList<JusticeUser> csos)
        {
            var csoIndex = _randomNumberGenerator.Generate(csos.Count);
            var cso = csos[csoIndex-1];
            return cso;
        }
    }
}
