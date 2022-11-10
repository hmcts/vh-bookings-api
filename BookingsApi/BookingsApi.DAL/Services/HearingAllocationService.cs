using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Helpers;
using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BookingsApi.DAL.Services
{
    public interface IHearingAllocationService
    {
        Task<JusticeUser> AllocateAutomatically(Guid hearingId);
    }

    public class HearingAllocationService : IHearingAllocationService
    {
        private readonly BookingsDbContext _context;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly AllocateHearingConfiguration _configuration;

        public HearingAllocationService(
            BookingsDbContext context, 
            IRandomNumberGenerator randomNumberGenerator,
            IOptions<AllocateHearingConfiguration> configuration)
        {
            _context = context;
            _randomNumberGenerator = randomNumberGenerator;
            _configuration = configuration.Value;
        }
        
        public async Task<JusticeUser> AllocateAutomatically(Guid hearingId)
        {
            var hearing = await _context.VideoHearings.SingleOrDefaultAsync(x => x.Id == hearingId);
            if (hearing == null)
            {
                throw new DomainRuleException("HearingNotFound",
                    $"Hearing {hearingId} not found");
            }

            var allocations = _context.Allocations.Where(a => a.HearingId == hearing.Id).ToList();
            if (allocations.Any())
            {
                throw new DomainRuleException("HearingAlreadyAllocated",
                    $"Hearing {hearing.Id} has already been allocated");
            }

            var hearingStartTime = hearing.ScheduledDateTime;
            var hearingEndTime = hearing.ScheduledEndTime;

            if (hearingStartTime.Date != hearingEndTime.Date)
            {
                throw new DomainRuleException("AllocationNotSupported",
                    $"Unable to allocate to hearing {hearing.Id}, hearings which span multiple days are not currently supported");
            }

            var cso = SelectCso(hearingStartTime, hearingEndTime);
            if (cso == null)
            {
                throw new DomainRuleException("NoCsosAvailable",
                    $"Unable to allocate to hearing {hearingId}, no CSOs available");
            }

            return cso;
        }

        private JusticeUser SelectCso(DateTime hearingStartTime, DateTime hearingEndTime)
        {
            var availableCsos = GetAvailableCsos(hearingStartTime, hearingEndTime);

            if (!availableCsos.Any())
            {
                return null;
            }

            if (availableCsos.Count() == 1)
            {
                return availableCsos.SingleOrDefault();
            }

            var csos = availableCsos
                .Select(c => new
                {
                    Cso = c,
                    AllocationCount = c.Allocations?.Count ?? 0
                })
                .ToList();
            
            var lowestAllocationCount = csos.Min(c => c.AllocationCount);
            var csosWithFewestAllocations = csos.Where(c => c.AllocationCount == lowestAllocationCount).ToList();

            if (csosWithFewestAllocations.Count == 1)
            {
                return csosWithFewestAllocations.Single().Cso;
            }

            return SelectRandomly(csosWithFewestAllocations.Select(c => c.Cso).ToList());
        }

        private IEnumerable<JusticeUser> GetAvailableCsos(DateTime hearingStartTime, DateTime hearingEndTime)
        {
            var availableCsos = new List<JusticeUser>();

            var users = _context.JusticeUsers
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations).ThenInclude(a => a.Hearing)
                .Include(u => u.UserRole)
                .Where(u => u.UserRoleId == (int)UserRoleId.Vho)
                .ToList();
            
            foreach (var justiceUser in users)
            {
                var workHoursFallingOnThisDay = justiceUser.VhoWorkHours
                    .FirstOrDefault(h => h.SystemDayOfWeek == hearingStartTime.DayOfWeek);
                if (workHoursFallingOnThisDay == null)
                {
                    continue;
                }

                if (justiceUser.VhoNonAvailability.Any(na => 
                        !na.Deleted && new DateRange(na.StartTime, na.EndTime).IsInRange(hearingStartTime, hearingEndTime)))
                {
                    continue;
                }

                if (justiceUser.Allocations.Any(a => (hearingStartTime - a.Hearing.ScheduledDateTime).TotalMinutes < _configuration.MinimumGapBetweenHearingsInMinutes))
                {
                    continue;
                }

                var concurrentAllocations = GetConcurrentAllocations(hearingStartTime, hearingEndTime, justiceUser.Allocations);
                if (concurrentAllocations.Count > _configuration.MaximumConcurrentHearings)
                {
                    continue;
                }

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

        private static IList<DateRange> GetConcurrentAllocations(DateTime hearingStartTime, DateTime hearingEndTime, IList<Allocation> allocations)
        {
            var hearing = new DateRange(hearingStartTime, hearingEndTime);
            var concurrentAllocations = new List<DateRange>();
            var index = 0;
            var allocationsToCheck = allocations
                .Select(a => new DateRange(a.Hearing.ScheduledDateTime, a.Hearing.ScheduledEndTime))
                .Union(new List<DateRange>{ hearing })
                .OrderBy(a => a.StartDate)
                .ToList();
            
            foreach (var allocation in allocationsToCheck)
            {
                if (index > 0)
                {
                    var previousAllocation = allocationsToCheck[index - 1];
                    var isConcurrent = allocation.IsInRange(previousAllocation);
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

            return concurrentAllocations;
        }

        private JusticeUser SelectRandomly(IList<JusticeUser> csos)
        {
            var csoIndex = _randomNumberGenerator.Generate(1, csos.Count);
            var cso = csos[csoIndex-1];
            return cso;
        }
    }
}
