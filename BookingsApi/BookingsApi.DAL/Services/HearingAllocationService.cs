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
using Microsoft.ApplicationInsights.Extensibility.Implementation;
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
            Console.WriteLine($"Allocation settings: {System.Text.Json.JsonSerializer.Serialize(_configuration)}");

            return new JusticeUser();
            
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
            var availableCsos = GetAvailableCsos(hearingStartTime, hearingEndTime).ToArray();

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

        private List<JusticeUser> GetAvailableCsos(DateTime hearingStartTime, DateTime hearingEndTime)
        {
            var availableCsos = new List<JusticeUser>();

            var csos = _context.JusticeUsers
                .Include(u => u.UserRole)
                .Where(u => u.UserRoleId == (int)UserRoleId.Vho)
                .ToList();

            var csoUserIds = csos.Select(u => u.Id).ToList();

            var workHours = _context.VhoWorkHours
                .Where(wh => csoUserIds.Contains(wh.JusticeUserId))
                .AsEnumerable()
                .Where(wh => wh.SystemDayOfWeek == hearingStartTime.DayOfWeek)
                .ToList();

            var nonAvailabilities = _context.VhoNonAvailabilities
                .Where(na => csoUserIds.Contains(na.JusticeUserId))
                .Where(na => na.StartTime <= hearingEndTime)
                .Where(na => hearingStartTime <= na.EndTime)
                .Where(na => !na.Deleted)
                .ToList();

            var allocations = _context.Allocations
                .Include(a => a.Hearing)
                .Where(a => csoUserIds.Contains(a.JusticeUserId))
                // Only those that end on or after this hearing's start time, plus our minimum allowed gap
                .Where(a => a.Hearing.ScheduledDateTime.AddMinutes(a.Hearing.ScheduledDuration + _configuration.MinimumGapBetweenHearingsInMinutes) >= hearingStartTime)
                .ToList();

            var csoAvailabilities = csos.Select(c => new
            {
                CsoUser = c,
                WorkHours = workHours.Where(wh => wh.JusticeUserId == c.Id).ToList(),
                NonAvailabilities = nonAvailabilities.Where(na => na.JusticeUserId == c.Id).ToList(),
                Allocations = allocations.Where(a => a.JusticeUserId == c.Id).ToList()
            }).ToList();
            
            foreach (var cso in csoAvailabilities)
            {
                var workHoursForThisHearing = cso.WorkHours.FirstOrDefault();
                if (workHoursForThisHearing == null)
                {
                    continue;
                }
                
                if (cso.NonAvailabilities.Any())
                {
                    continue;
                }

                var gapBetweenHearingsIsInsufficient = cso.Allocations.Any(a => (hearingStartTime - a.Hearing.ScheduledDateTime).TotalMinutes < _configuration.MinimumGapBetweenHearingsInMinutes);
                if (gapBetweenHearingsIsInsufficient)
                {
                    continue;
                }

                var concurrentAllocatedHearings = CountConcurrentAllocatedHearings(hearingStartTime, hearingEndTime, cso.Allocations);
                if (concurrentAllocatedHearings > _configuration.MaximumConcurrentHearings)
                {
                    continue;
                }

                var workHourStartTime = workHoursForThisHearing.StartTime;
                var workHourEndTime = workHoursForThisHearing.EndTime;

                if ((workHourStartTime <= hearingStartTime.TimeOfDay || _configuration.AllowHearingToStartBeforeWorkStartTime) && 
                    (workHourEndTime >= hearingEndTime.TimeOfDay || _configuration.AllowHearingToEndAfterWorkEndTime))
                {
                    availableCsos.Add(cso.CsoUser);
                }
            }

            return availableCsos;
        }

        private static int CountConcurrentAllocatedHearings(DateTime hearingStartTime, DateTime hearingEndTime, IList<Allocation> allocations)
        {
            var hearing = new DateRange(hearingStartTime, hearingEndTime);
            var hearingsToCheck = allocations
                .Select(a => new DateRange(a.Hearing.ScheduledDateTime, a.Hearing.ScheduledEndTime))
                .Union(new List<DateRange>{ hearing })
                .OrderBy(a => a.StartDate)
                .ToList();
    
            var minEndTime = hearingsToCheck.Min(a => a.EndDate);
            var count = hearingsToCheck.Count(a => a.StartDate < minEndTime);

            return count;
        }

        private JusticeUser SelectRandomly(IList<JusticeUser> csos)
        {
            var csoIndex = _randomNumberGenerator.Generate(1, csos.Count);
            var cso = csos[csoIndex-1];
            return cso;
        }
    }
}
