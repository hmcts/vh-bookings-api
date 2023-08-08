using BookingsApi.Common.Services;
using BookingsApi.DAL.Dtos;
using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Helper;
using BookingsApi.Domain.Validations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookingsApi.DAL.Services
{
    public interface IHearingAllocationService
    {
        Task<JusticeUser> AllocateAutomatically(Guid hearingId);
        Task<JusticeUser> AllocateManually(Guid hearingId, Guid justiceUserCsoId);
        Task DeallocateFromUnavailableHearings(Guid justiceUserId);
        void CheckAndDeallocateHearing(Hearing hearing);
        Task<List<VideoHearing>> AllocateHearingsToCso(List<Guid> postRequestHearings, Guid postRequestCsoId);
        List<HearingAllocationResultDto> CheckForAllocationClashes(List<VideoHearing> hearings);
   
    }

    public class HearingAllocationService : IHearingAllocationService
    {
        private readonly BookingsDbContext _context;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly AllocateHearingConfiguration _configuration;
        private readonly ILogger<HearingAllocationService> _logger;

        public HearingAllocationService(
            BookingsDbContext context, 
            IRandomNumberGenerator randomNumberGenerator,
            IOptions<AllocateHearingConfiguration> configuration,
            ILogger<HearingAllocationService> logger)
        {
            _context = context;
            _randomNumberGenerator = randomNumberGenerator;
            _configuration = configuration.Value;
            _logger = logger;
        }

        /// <summary>
        /// Allocate a list of hearings to a passed CSO.
        /// </summary>
        /// <param name="postRequestHearings">List of hearings to be allocated</param>
        /// <param name="postRequestCsoId">CSO JusticeUser Id</param>
        /// <exception cref="DomainRuleException"></exception>
        public async Task<List<VideoHearing>> AllocateHearingsToCso(List<Guid> postRequestHearings, Guid postRequestCsoId)
        {
            try
            {
                foreach (Guid guid in postRequestHearings)
                {
                    await AllocateManually(guid, postRequestCsoId);
                }
                
                return await GetListOfHearings(postRequestHearings);
            }
            catch (DomainRuleException e)
            {
                throw new DomainRuleException("NoCsosAvailable",
                    e.Message);
            }
        }

        public List<HearingAllocationResultDto> CheckForAllocationClashes(List<VideoHearing> hearings)
        {
            var vhoUsernamesConcurrencyCount = BuildUsernameConcurrencyCountDictionary(hearings);
            var dto = hearings.Select(x =>
            {
                bool? hasWorkHoursClash = null;
                bool? hasNonAvailabilityClash = null;
                int? concurrentHearingsCount = null;
                if (x.HearingVenue.IsWorkAllocationEnabled && x.AllocatedTo != null)
                {
                    hasWorkHoursClash =
                        !x.AllocatedTo.IsDateBetweenWorkingHours(x.ScheduledDateTime, x.ScheduledEndTime, _configuration);
                    hasNonAvailabilityClash = 
                        x.AllocatedTo.IsDuringNonAvailableHours(x.ScheduledDateTime, x.ScheduledEndTime);
                    if (vhoUsernamesConcurrencyCount.TryGetValue(x.AllocatedTo.Username, out var count))
                    {
                        concurrentHearingsCount = count;
                    }
                }
                
                return new HearingAllocationResultDto
                {
                    HearingId = x.Id,
                    CaseNumber = x.HearingCases.FirstOrDefault()?.Case.Number,
                    CaseType = x.CaseType.Name,
                    ScheduledDateTime = x.ScheduledDateTime,
                    Duration = x.ScheduledDuration,
                    AllocatedCso = VideoHearingHelper.AllocatedVho(x),
                    HasWorkHoursClash = hasWorkHoursClash,
                    ConcurrentHearingsCount = concurrentHearingsCount,
                    HasNonAvailabilityClash = hasNonAvailabilityClash
                        
                };
            }).ToList();
            return dto;
        }
        
        /// <summary>
        /// Allocate automatically a hearing to a random CSO
        /// </summary>
        /// <param name="hearingId">hearing Id to be allocated</param>
        /// <returns>CSO allocated User</returns>
        /// <exception cref="DomainRuleException"></exception>
        public async Task<JusticeUser> AllocateAutomatically(Guid hearingId)
        {
            var hearing = await GetHearing(hearingId);
            
            // only in automatic allocation we check if hearing has been allocated
            if (hearing.Allocations.Any())
            {
                throw new DomainRuleException("HearingAlreadyAllocated",
                    $"Hearing {hearing.Id} has already been allocated");
            }
            
            var cso = SelectCso(hearing);
            if (cso == null)
            {
                throw new DomainRuleException("NoCsosAvailable",
                    $"Unable to allocate to hearing {hearingId}, no CSOs available");
            }
            
            await AllocateHearingToCso(cso, hearing);

            return cso;
        }
        
        /// <summary>
        /// Allocate manually to a passed CSO
        /// </summary>
        /// <param name="hearingId">hearing Id to be allocated</param>
        /// <param name="justiceUserCsoId">CSO JusticeUser Id</param>
        /// <returns>CSO allocated User</returns>
        /// <exception cref="DomainRuleException"></exception>
        public async Task<JusticeUser> AllocateManually(Guid hearingId, Guid justiceUserCsoId)
        {
            VideoHearing hearing = await GetHearing(hearingId);
            
            var allocations = _context.Allocations.Where(a => a.HearingId == hearing.Id).ToList();

            if (allocations.Any())
            {
                // we need to unallocate the hearing and allocate to the new user
                hearing.Deallocate();
                _logger.LogInformation("Deallocated hearing {hearingId}", hearing.Id);
            }
            var cso = GetCso(justiceUserCsoId);
            if (cso == null)
            {
                throw new DomainRuleException("CsoNotFound",
                    $"Unable to allocate to hearing {hearingId}, with CSO {justiceUserCsoId}");
            }

            await AllocateHearingToCso(cso, hearing);

            return cso;
        }
        
        private async Task<List<VideoHearing>> GetListOfHearings(List<Guid> hearingIds)
        {
            return await _context.VideoHearings
                .Include(h => h.CaseType)
                .Include(h => h.HearingType)
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser).ThenInclude(x => x.VhoWorkHours)
                .Include(h => h.HearingCases).ThenInclude(hc => hc.Case)
                .Include(h => h.HearingVenue)
                .Where(x => hearingIds.Contains(x.Id))
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

        private async Task<VideoHearing> GetHearing(Guid hearingId)
        {
            var hearing = await _context.VideoHearings
                .Include(h => h.CaseType)
                .Include(h => h.HearingType)
                .Include(h => h.HearingCases).ThenInclude(hc => hc.Case)
                .Include(h => h.Allocations).ThenInclude(a => a.JusticeUser).ThenInclude(x => x.VhoWorkHours)
                .Include(h => h.Participants).ThenInclude(i => i.HearingRole).ThenInclude(aa => aa.UserRole)
                .Include(h => h.HearingVenue)
                .AsSplitQuery().SingleOrDefaultAsync(x => x.Id == hearingId);
            if (hearing == null)
            {
                throw new DomainRuleException("HearingNotFound",
                    $"Hearing {hearingId} not found");
            }

            return hearing;
        }

        private async Task AllocateHearingToCso(JusticeUser cso, VideoHearing hearing)
        {
            hearing.Allocations.Add(new Allocation
            {
                Hearing = hearing,
                JusticeUser = cso
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation("Allocated hearing {HearingId} to cso {Cso}", hearing.Id, cso.Username);
        }

        /// <summary>
        /// Deallocates the user from any hearings they are no longer available for
        /// </summary>
        /// <param name="justiceUserId"></param>
        /// <exception cref="DomainRuleException"></exception>
        public async Task DeallocateFromUnavailableHearings(Guid justiceUserId)
        {
            var user = await _context.JusticeUsers
                .Include(x => x.Allocations).ThenInclude(x => x.Hearing)
                .Include(x => x.VhoWorkHours)
                .Include(x => x.VhoNonAvailability)
                .SingleOrDefaultAsync(x => x.Id == justiceUserId);
            if (user == null)
            {
                throw new DomainRuleException("JusticeUserNotFound",
                    $"Justice user {justiceUserId} not found");
            }

            var allocations = user.Allocations
                .Where(x => x.Hearing.ScheduledDateTime > DateTime.UtcNow)
                .ToList();
            
            foreach (var hearing in allocations.Select(allocation => allocation.Hearing).ToList())
            {
                if (!user.IsAvailable(hearing.ScheduledDateTime, hearing.ScheduledEndTime, _configuration))
                {
                    hearing.Deallocate();
                    _logger.LogInformation("Deallocated hearing {HearingId}", hearing.Id);
                }
            }
        }
        
        /// <summary>
        /// Deallocates the user from the hearing if they are not available
        /// </summary>
        /// <param name="hearing"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void CheckAndDeallocateHearing(Hearing hearing)
        {
            var allocatedUser = hearing.AllocatedTo;
            if (allocatedUser == null)
            {
                return;
            }
            
            if (!allocatedUser.IsAvailable(hearing.ScheduledDateTime, hearing.ScheduledEndTime, _configuration))
            {
                hearing.Deallocate();
                _logger.LogInformation("Deallocated hearing {HearingId}", hearing.Id);
            }
        }

        private JusticeUser SelectCso(Hearing hearing)
        {
            var availableCsos = GetAvailableCsos(hearing);

            if (!availableCsos.Any())
            {
                return null;
            }

            if (availableCsos.Count == 1)
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

        private List<JusticeUser> GetAvailableCsos(Hearing hearing)
        {
            var csos = _context.JusticeUsers
                .Include(u => u.JusticeUserRoles).ThenInclude(ur => ur.UserRole)
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations).ThenInclude(a => a.Hearing)
                .Where(ju => ju.JusticeUserRoles.Any(jur => jur.UserRole.Id == (int)UserRoleId.Vho))
                .ToList();

            return csos
                .Where(cso => hearing.CanAllocate(cso, _configuration))
                .ToList();
        }
        
        private JusticeUser GetCso(Guid id)
        {
            var cso = _context.JusticeUsers
                .Include(u => u.JusticeUserRoles).ThenInclude(ur => ur.UserRole)
                .Where(u => u.Id == id)
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations).ThenInclude(a => a.Hearing)
                .FirstOrDefault();

            return cso;
        }

        private JusticeUser SelectRandomly(IList<JusticeUser> csos)
        {
            var csoIndex = _randomNumberGenerator.Generate(1, csos.Count);
            var cso = csos[csoIndex-1];
            return cso;
        }
        
        private static Dictionary<string, int> BuildUsernameConcurrencyCountDictionary(List<VideoHearing> hearings)
        {
            // builds a dictionary of usernames and a boolean indicating whether the user has 
            // concurrent hearings exceeding the 'concurrencyLimit'
            
            // get a distinct list of all users for the current collection of hearings
            var vhoUsernames = hearings.Where(x => x.AllocatedTo != null).Select(x => x.AllocatedTo.Username).Distinct();
            
            var vhoUsernamesConcurrencyExceeded = new Dictionary<string, int>();

            // for each username, determine if, for the current collection of hearings, it has more than 3 overlapping hearings
            foreach (var username in vhoUsernames)
            {
                // get all the hearings allocated to the username
                var hearingsForVho = hearings.Where(x => x.AllocatedTo?.Username == username)
                    .OrderBy(hearing => hearing.ScheduledDateTime).ToList();

                // there can't be an overlap if there's only 1 hearing, return false for this username
                if (hearingsForVho.Count <= 1)
                {
                    vhoUsernamesConcurrencyExceeded.Add(username, 0);
                    return vhoUsernamesConcurrencyExceeded;
                }
                
                // if there is more than 1 hearing, calculate number that are concurrent and add to dictionay
                vhoUsernamesConcurrencyExceeded.Add(username, CalculateNumberOfConcurrentHearings(hearingsForVho));
            }

            return vhoUsernamesConcurrencyExceeded;
        }

        private static int CalculateNumberOfConcurrentHearings(List<VideoHearing> hearingsForVho)
        {
            var concurrentHearingsCount = 0;
            
            // for each hearing assigned to this VHO
            for (var i = 0; i < hearingsForVho.Count; i++)
            {
                // get the current hearing
                var currentHearing = hearingsForVho[i];

                // if the current index + 1 is less than the number of hearings, there must be a 'nextHearing'
                if (i + 1 < hearingsForVho.Count)
                {
                    var nextHearing = hearingsForVho[i + 1];
                    if (IsHearingConcurrent(currentHearing, nextHearing))
                    {
                        concurrentHearingsCount++;
                    }
                }
                // if there is no next hearing, there has to be a previous hearing because we have already
                // determined that there are 2 or more hearings
                else
                {
                    var previousHearing = hearingsForVho[i - 1];
                    if (IsHearingConcurrent(previousHearing, currentHearing))
                    {
                        concurrentHearingsCount++;
                    }
                }
            }

            return concurrentHearingsCount;
        }

        private static bool IsHearingConcurrent(Hearing hearing, Hearing comparisonHearing)
        {
            // if the hearing's start time plus its duration is greater than the start time of
            // the comparison hearing, they are considered to be concurrent
            return hearing.ScheduledDateTime.AddMinutes(hearing.ScheduledDuration) >
                   comparisonHearing.ScheduledDateTime;
        }
    }
}