using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Services
{
    public interface IHearingAllocationService
    {
        Task<JusticeUser> AllocateCso(Guid hearingId);
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
    
    public class HearingAllocationService : IHearingAllocationService
    {
        private readonly BookingsDbContext _context;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly AllocateCsoConfiguration _configuration;

        public HearingAllocationService(
            BookingsDbContext context, 
            IRandomNumberGenerator randomNumberGenerator,
            AllocateCsoConfiguration configuration)
        {
            _context = context;
            _randomNumberGenerator = randomNumberGenerator;
            _configuration = configuration;
        }
        
        public async Task<JusticeUser> AllocateCso(Guid hearingId)
        {
            var hearing = await _context.VideoHearings.SingleOrDefaultAsync(x => x.Id == hearingId);
            if (hearing == null)
            {
                throw new ArgumentException($"Hearing {hearingId} not found");   
            }

            var hearingStartTime = hearing.ScheduledDateTime;
            var hearingEndTime = hearing.ScheduledDateTime.AddMinutes(hearing.ScheduledDuration);

            if (hearingStartTime.Date != hearingEndTime.Date)
            {
                throw new NotSupportedException($"Unable to allocate to hearing {hearing.Id}, hearings which span multiple days are not currently supported");
            }
            
            var availableCsos = GetAvailableCsos(hearingStartTime, hearingEndTime);

            if (!availableCsos.Any())
            {
                throw new InvalidOperationException($"Unable to allocate to hearing {hearingId}, no CSOs available");
            }

            if (availableCsos.Count() == 1)
            {
                return availableCsos.SingleOrDefault();
            }

            var csosWithNoAllocations = new List<JusticeUser>();
            var csosWithOneAllocation = new List<JusticeUser>();
            var csosWithTwoAllocations = new List<JusticeUser>();

            foreach (var cso in availableCsos)
            {
                var allocations = _context.Allocations
                    .Where(a => a.JusticeUserId == cso.Id)
                    .ToList();

                if (!allocations.Any())
                {
                    csosWithNoAllocations.Add(cso);
                }

                if (allocations.Count == 1)
                {
                    csosWithOneAllocation.Add(cso);
                }

                if (allocations.Count == 2)
                {
                    csosWithTwoAllocations.Add(cso);
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

            if (csosWithOneAllocation.Count == 1)
            {
                return csosWithOneAllocation.SingleOrDefault();
            }

            if (csosWithOneAllocation.Count > 1)
            {
                return AllocateRandomly(csosWithOneAllocation);
            }

            if (csosWithTwoAllocations.Count == 1)
            {
                return csosWithTwoAllocations.SingleOrDefault();
            }

            if (csosWithTwoAllocations.Count > 1)
            {
                return AllocateRandomly(csosWithTwoAllocations);
            }

            throw new InvalidOperationException($"Unable to allocate to hearing {hearingId}, no CSOs available");
        }

        private IEnumerable<JusticeUser> GetAvailableCsos(DateTime hearingStartTime, DateTime hearingEndTime)
        {
            var availableCsos = new List<JusticeUser>();

            foreach (var justiceUser in _context.JusticeUsers.Where(u => u.UserRoleId == (int)UserRoleId.Vho).ToList())
            {
                var workHoursFallingOnThisDay = justiceUser.VhoWorkHours
                    .FirstOrDefault(h => 
                        DayOfWeekIdToSystemDayOfWeek(h.DayOfWeekId) == hearingStartTime.DayOfWeek); // Note we are assuming here that a hearing will not span multiple days
                if (workHoursFallingOnThisDay == null)
                {
                    continue;
                }

                // eg if the user is unavailable from 15:30 - 18:00
                // and the hearing is scheduled from 15:00 - 16:00
                // then they are not available for the entire hearing
                
                if (justiceUser.VhoNonAvailability.Any(na => 
                        (na.StartTime <= hearingStartTime && na.EndTime >= hearingStartTime) ||
                        (na.StartTime <= hearingEndTime && na.StartTime >= hearingStartTime)))
                {
                    continue;
                }

                // If the hearing is less than 30 minutes of another allocation, we cannot allocate to that cso
                var allocations = _context.Allocations
                    .Where(a => a.JusticeUserId == justiceUser.Id)
                    .ToList();
                
                if (allocations.Any(a => (hearingStartTime - a.Hearing.ScheduledDateTime).TotalMinutes < _configuration.MinimumGapBetweenHearingsInMinutes))
                {
                    continue;
                }
                
                // Cso can have a maximum of 3 concurrent hearings
                var hearing = new DateRange(hearingStartTime, hearingEndTime);
                var concurrentAllocations = new List<DateRange>();
                var index = 0;
                var allocationsToCheck = allocations
                    .Select(a => new DateRange(a.Hearing.ScheduledDateTime, a.Hearing.ScheduledDateTime.AddMinutes(a.Hearing.ScheduledDuration)))
                    .Union(new List<DateRange>{ hearing })
                    .OrderBy(a => a.StartDate)
                    .ToList();
                foreach (var allocation in allocationsToCheck)
                {
                    if (index > 0)
                    {
                        var previousAllocation = allocationsToCheck[index - 1];
                        var isConcurrent = DatesAreInRange(allocation, previousAllocation);
                        if (isConcurrent)
                        {
                            concurrentAllocations.Add(allocation);
                            if (!concurrentAllocations.Contains(previousAllocation))
                            {
                                concurrentAllocations.Add(previousAllocation);
                            }
                        }
                    }
                    
                    index++;
                }

                if (concurrentAllocations.Count > 3)
                {
                    continue;
                }
                
                // eg if the user works from 15:30 - 18:00
                // and the hearing is scheduled from 15:00 - 16:00
                // then they are not available for the entire hearing
                
                var workHourStartTime = workHoursFallingOnThisDay.StartTime;
                var workHourEndTime = workHoursFallingOnThisDay.EndTime;

                if ((workHourStartTime <= hearingStartTime.TimeOfDay || _configuration.AllowHearingToStartBeforeWorkStartTime) && 
                    (workHourEndTime >= hearingEndTime.TimeOfDay || _configuration.AllowHearingToEndAfterWorkEndTime))
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
            var csoIndex = _randomNumberGenerator.Generate(1, csos.Count);
            var cso = csos[csoIndex-1];
            return cso;
        }

        private bool DatesAreInRange(DateRange dateRange1, DateRange dateRange2)
        {
            return dateRange1.StartDate <= dateRange2.EndDate && dateRange2.StartDate <= dateRange1.EndDate;
        }
    }
}
